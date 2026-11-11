using System.Security.Cryptography;
using System.Text;
using LABOFMUTANTAPI.Context;
using LABOFMUTANTAPI.Dto;
using LABOFMUTANTAPI.Entities;
using LABOFMUTANTAPI.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LABOFMUTANTAPI.Services.Services;
public sealed class ComentariosService(AplicacionBdContext database) : IComentariosService
{
    private static ComentarioDto Dto(Comentario value) => new(value.Id, value.Nombre, value.Texto, value.CreadoUtc);
    public async Task<PaginaComentariosDto> Listar(long? antesDe, CancellationToken cancellation)
    {
        var query = database.Comentarios.AsNoTracking();
        var total = await query.CountAsync(cancellation);
        var personas = await query.Select(x => x.AutorId).Distinct().CountAsync(cancellation);
        var items = await query.Where(x => !antesDe.HasValue || x.Id < antesDe.Value).OrderByDescending(x => x.Id).Take(21).ToListAsync(cancellation);
        var visible = items.Take(20).Select(Dto).ToList();
        return new(total, personas, visible, items.Count > 20 ? visible[^1].Id : null);
    }
    public async Task<ComentarioDto> Publicar(string token, NuevoComentarioDto nuevo, CancellationToken cancellation)
    {
        var nombre = (nuevo.Nombre ?? "").Trim(); var texto = (nuevo.Texto ?? "").Trim();
        if (nuevo.IdCliente == Guid.Empty || nombre.Length is < 1 or > 40 || texto.Length is < 1 or > 600)
            throw new ArgumentException("Escribe un nombre de hasta 40 caracteres y un comentario de hasta 600.");
        var autor = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        var existing = await database.Comentarios.AsNoTracking().SingleOrDefaultAsync(x => x.AutorId == autor && x.IdCliente == nuevo.IdCliente, cancellation);
        if (existing is not null) return Dto(existing);
        var value = new Comentario { AutorId = autor, IdCliente = nuevo.IdCliente, Nombre = nombre, Texto = texto, CreadoUtc = DateTimeOffset.UtcNow };
        database.Comentarios.Add(value);
        try { await database.SaveChangesAsync(cancellation); }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            database.Entry(value).State = EntityState.Detached;
            return Dto(await database.Comentarios.AsNoTracking().SingleAsync(x => x.AutorId == autor && x.IdCliente == nuevo.IdCliente, cancellation));
        }
        return Dto(value);
    }
}
