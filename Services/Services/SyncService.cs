using System.Text.Json;
using LABOFMUTANTAPI.Context;
using LABOFMUTANTAPI.Dto;
using LABOFMUTANTAPI.Entities;
using LABOFMUTANTAPI.Services.Interfaces;

namespace LABOFMUTANTAPI.Services.Services;
public sealed class SyncService(AplicacionDataContext context) : ISyncService
{
    private const long MaxSafeInteger = 9007199254740991;
    private static bool ValidId(long? id) => id is > 0 and <= MaxSafeInteger;
    private static void Validate(SyncOperationDto operation)
    {
        if (operation is null) throw new ArgumentException("Cambio inválido.");
        if (!ValidId(operation.Seq) || operation.CreatedAt < 0) throw new ArgumentException("Secuencia o fecha inválida.");
        switch (operation.Kind)
        {
            case "set":
                if (operation.Key is not ("profile" or "prefs" or "stats") || operation.Value?.ValueKind != JsonValueKind.Object)
                    throw new ArgumentException("Dato de perfil, preferencias o progreso inválido.");
                if (operation.Key == "profile" && operation.Value.Value.TryGetProperty("name", out var name) &&
                    (name.ValueKind != JsonValueKind.String || (name.GetString()?.Length ?? 0) > 40))
                    throw new ArgumentException("Nombre de perfil inválido.");
                break;
            case "note.put":
                if (operation.Note is null || !ValidId(operation.Note.Id) || string.IsNullOrWhiteSpace(operation.Note.Text) ||
                    operation.Note.Text.Length > 600 || operation.Note.CreatedAt < 0) throw new ArgumentException("Nota inválida.");
                break;
            case "note.delete":
                if (!ValidId(operation.NoteId)) throw new ArgumentException("Identificador de nota inválido.");
                break;
            case "reset": break;
            default: throw new ArgumentException("Tipo de cambio no admitido.");
        }
    }
    public async Task<SyncResponseDto> Receive(string token, SyncRequestDto request, CancellationToken cancellation)
    {
        if (request.Operations is null || request.Operations.Count is < 1 or > 50) throw new ArgumentException("Envía entre 1 y 50 cambios.");
        long previous = 0;
        foreach (var operation in request.Operations)
        {
            Validate(operation);
            if (operation.Seq <= previous) throw new ArgumentException("Los cambios deben llegar ordenados y sin secuencias repetidas.");
            previous = operation.Seq;
        }
        var state = await context.Update(token, state =>
        {
            foreach (var operation in request.Operations)
            {
                // Un reintento no vuelve a aplicar cambios ya confirmados.
                if (operation.Seq <= state.LastSequence) continue;
                switch (operation.Kind)
                {
                    case "set": state.Data[operation.Key!] = operation.Value!.Value.Clone(); break;
                    case "note.put": state.Notes[operation.Note!.Id] = new Note { Id = operation.Note.Id, Text = operation.Note.Text, CreatedAt = operation.Note.CreatedAt }; break;
                    case "note.delete": state.Notes.Remove(operation.NoteId!.Value); break;
                    case "reset": state.Data.Clear(); state.Notes.Clear(); break;
                }
                state.LastSequence = operation.Seq;
            }
        }, cancellation);
        return new(request.Operations.Select(operation => operation.Seq).ToArray(), state.LastSequence, state.UpdatedAt);
    }
    public Task<DeviceState> Get(string token, CancellationToken cancellation) => context.Get(token, cancellation);
}
