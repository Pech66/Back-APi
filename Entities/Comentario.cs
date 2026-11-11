namespace LABOFMUTANTAPI.Entities;

public sealed class Comentario
{
    public long Id { get; set; }
    public Guid IdCliente { get; set; }
    public string AutorId { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Texto { get; set; } = "";
    public DateTimeOffset CreadoUtc { get; set; }
}
