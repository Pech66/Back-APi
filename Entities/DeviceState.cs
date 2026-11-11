using System.Text.Json;
namespace LABOFMUTANTAPI.Entities;
public sealed class DeviceState
{
    public long LastSequence { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public Dictionary<string, JsonElement> Data { get; set; } = [];
    public Dictionary<long, Note> Notes { get; set; } = [];
}
public sealed class Note
{
    public long Id { get; set; }
    public string Text { get; set; } = "";
    public long CreatedAt { get; set; }
}
