using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace LABOFMUTANTAPI.Dto;
public sealed class SyncRequestDto
{
    [Required, MinLength(1), MaxLength(50)]
    public List<SyncOperationDto> Operations { get; set; } = [];
}
public sealed class SyncOperationDto
{
    public long Seq { get; set; }
    public string Kind { get; set; } = "";
    public string? Key { get; set; }
    public JsonElement? Value { get; set; }
    public NoteDto? Note { get; set; }
    public long? NoteId { get; set; }
    public long CreatedAt { get; set; }
}
public sealed class NoteDto
{
    public long Id { get; set; }
    public string Text { get; set; } = "";
    public long CreatedAt { get; set; }
}
