using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LABOFMUTANTAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LABOFMUTANTAPI.Context;
// Conserva el contrato de sincronización; la persistencia ahora es SQL Server.
public sealed class AplicacionDataContext(AplicacionBdContext db)
{
    private static string DeviceId(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    private async Task Lock(string id, string mode, CancellationToken cancellation)
    {
        // El bloqueo pertenece a la transacción y también protege varias instancias de la API.
        await db.Database.ExecuteSqlInterpolatedAsync($@"
DECLARE @result int;
EXEC @result = sp_getapplock @Resource={"lom:" + id}, @LockMode={mode}, @LockOwner='Transaction', @LockTimeout=15000;
IF @result < 0 THROW 51000, 'No se pudo bloquear la sincronizacion del dispositivo.', 1;", cancellation);
    }
    private async Task<DeviceState> Read(string id, CancellationToken cancellation)
    {
        var device = await db.Dispositivos.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellation);
        if (device is null) return new();
        var state = new DeviceState { LastSequence = device.UltimaSecuencia, UpdatedAt = device.ActualizadoUtc };
        var profile = await db.Perfiles.AsNoTracking().SingleOrDefaultAsync(x => x.DispositivoId == id, cancellation);
        if (profile is not null) state.Data["profile"] = JsonSerializer.SerializeToElement(new { name = profile.Nombre });
        var prefs = await db.Preferencias.AsNoTracking().SingleOrDefaultAsync(x => x.DispositivoId == id, cancellation);
        if (prefs is not null) state.Data["prefs"] = JsonSerializer.SerializeToElement(new { torch = prefs.Linterna, guideTab = prefs.PestanaGuia });
        var stats = await db.Progresos.AsNoTracking().SingleOrDefaultAsync(x => x.DispositivoId == id, cancellation);
        if (stats is not null) state.Data["stats"] = JsonSerializer.SerializeToElement(new {
            visits = stats.Visitas, firstVisit = stats.PrimeraVisita, lastVisit = stats.UltimaVisita,
            timesSeen = stats.VecesDetectado, bestHidden = stats.MejorSigilo, keysTried = JsonSerializer.Deserialize<string[]>(stats.TeclasProbadasJson)
        });
        foreach (var note in await db.NotasDiario.AsNoTracking().Where(x => x.DispositivoId == id).ToListAsync(cancellation))
            state.Notes[note.IdLocal] = new Note { Id = note.IdLocal, Text = note.Texto, CreatedAt = note.CreadaUnixMs };
        return state;
    }
    public async Task<DeviceState> Get(string token, CancellationToken cancellation)
    {
        var id = DeviceId(token);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellation);
        await Lock(id, "Shared", cancellation);
        var state = await Read(id, cancellation);
        await transaction.CommitAsync(cancellation);
        return state;
    }
    private static string Text(JsonElement value, string key, int max, string fallback = "")
    {
        if (!value.TryGetProperty(key, out var item)) return fallback;
        if (item.ValueKind != JsonValueKind.String || item.GetString()!.Length > max) throw new ArgumentException("Texto inválido: " + key);
        return item.GetString()!;
    }
    private static long? Number(JsonElement value, string key, bool nullable = false)
    {
        if (!value.TryGetProperty(key, out var item) || item.ValueKind == JsonValueKind.Null) return nullable ? null : 0;
        if (item.ValueKind != JsonValueKind.Number || !item.TryGetInt64(out var number) || number < 0 || number > 9007199254740991) throw new ArgumentException("Número inválido: " + key);
        return number;
    }
    public async Task<DeviceState> Update(string token, Action<DeviceState> update, CancellationToken cancellation)
    {
        var id = DeviceId(token);
        await using var transaction = await db.Database.BeginTransactionAsync(cancellation);
        await Lock(id, "Exclusive", cancellation);
        var state = await Read(id, cancellation);
        update(state);
        state.UpdatedAt = DateTimeOffset.UtcNow;
        var device = await db.Dispositivos.SingleOrDefaultAsync(x => x.Id == id, cancellation);
        if (device is null) { device = new Dispositivo { Id = id }; db.Dispositivos.Add(device); }
        device.UltimaSecuencia = state.LastSequence; device.ActualizadoUtc = state.UpdatedAt;
        await db.SaveChangesAsync(cancellation);
        await db.Perfiles.Where(x => x.DispositivoId == id).ExecuteDeleteAsync(cancellation);
        await db.Preferencias.Where(x => x.DispositivoId == id).ExecuteDeleteAsync(cancellation);
        await db.Progresos.Where(x => x.DispositivoId == id).ExecuteDeleteAsync(cancellation);
        await db.NotasDiario.Where(x => x.DispositivoId == id).ExecuteDeleteAsync(cancellation);
        if (state.Data.TryGetValue("profile", out var profile))
            db.Perfiles.Add(new Perfil { DispositivoId = id, Nombre = Text(profile, "name", 40) });
        if (state.Data.TryGetValue("prefs", out var prefs)) {
            var tab = Text(prefs, "guideTab", 10, "t1");
            if (tab is not ("t1" or "t2" or "t3")) throw new ArgumentException("Pestaña de guía inválida.");
            var torch = !prefs.TryGetProperty("torch", out var light) ? true :
                light.ValueKind is JsonValueKind.True or JsonValueKind.False ? light.GetBoolean() : throw new ArgumentException("Estado de linterna inválido.");
            db.Preferencias.Add(new Preferencia { DispositivoId = id, Linterna = torch, PestanaGuia = tab });
        }
        if (state.Data.TryGetValue("stats", out var stats)) {
            var keys = stats.TryGetProperty("keysTried", out var list) ? list : JsonSerializer.SerializeToElement(Array.Empty<string>());
            if (keys.ValueKind != JsonValueKind.Array || keys.GetArrayLength() > 64 || keys.EnumerateArray().Any(x => x.ValueKind != JsonValueKind.String || x.GetString()!.Length > 40))
                throw new ArgumentException("Lista de controles inválida.");
            db.Progresos.Add(new Progreso { DispositivoId = id, Visitas = Number(stats, "visits")!.Value, PrimeraVisita = Number(stats, "firstVisit", true),
                UltimaVisita = Number(stats, "lastVisit", true), VecesDetectado = Number(stats, "timesSeen")!.Value,
                MejorSigilo = Number(stats, "bestHidden")!.Value, TeclasProbadasJson = keys.GetRawText() });
        }
        foreach (var note in state.Notes.Values)
            db.NotasDiario.Add(new NotaDiario { DispositivoId = id, IdLocal = note.Id, Texto = note.Text, CreadaUnixMs = note.CreatedAt });
        await db.SaveChangesAsync(cancellation);
        await transaction.CommitAsync(cancellation);
        return state;
    }
}
