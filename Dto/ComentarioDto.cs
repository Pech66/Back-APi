using System.ComponentModel.DataAnnotations;

namespace LABOFMUTANTAPI.Dto;
public sealed record NuevoComentarioDto(
    Guid IdCliente,
    [Required, StringLength(40, MinimumLength = 1)] string Nombre,
    [Required, StringLength(600, MinimumLength = 1)] string Texto);
public sealed record ComentarioDto(long Id, string Nombre, string Texto, DateTimeOffset CreadoUtc);
public sealed record PaginaComentariosDto(int TotalComentarios, int TotalParticipantes, IReadOnlyList<ComentarioDto> Comentarios, long? SiguienteAntesDe);
