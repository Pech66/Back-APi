using LABOFMUTANTAPI.Dto;
namespace LABOFMUTANTAPI.Services.Interfaces;
public interface IComentariosService
{
    Task<PaginaComentariosDto> Listar(long? antesDe, CancellationToken cancellation);
    Task<ComentarioDto> Publicar(string token, NuevoComentarioDto nuevo, CancellationToken cancellation);
}
