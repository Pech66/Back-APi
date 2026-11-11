using LABOFMUTANTAPI.Dto;
using LABOFMUTANTAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LABOFMUTANTAPI.Controllers;
[ApiController]
[Route("api/comentarios")]
public sealed class ComentariosController(IComentariosService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] long? antesDe, CancellationToken cancellation)
    {
        if (antesDe is <= 0) return BadRequest();
        return Ok(await service.Listar(antesDe, cancellation));
    }
    [HttpPost]
    [RequestSizeLimit(8192)]
    public async Task<IActionResult> Post([FromBody] NuevoComentarioDto nuevo, CancellationToken cancellation)
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) || !Guid.TryParseExact(header[7..], "D", out var token))
            return Unauthorized(new { message = "Falta el identificador del dispositivo." });
        try { return Ok(await service.Publicar(token.ToString("D"), nuevo, cancellation)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
