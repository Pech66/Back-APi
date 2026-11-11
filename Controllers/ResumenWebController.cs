using LABOFMUTANTAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LABOFMUTANTAPI.Controllers;

[ApiController]
[Route("api/sync/resumen")]
public sealed class ResumenWebController(AplicacionBdContext database) : ControllerBase
{
    // La PWA ya guarda sus notas mediante POST /api/sync.
    // Solo se devuelven cifras, nunca nombres, textos ni identificadores.
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        var notas = database.NotasDiario.AsNoTracking();
        var participantes = await notas.Select(x => x.DispositivoId).Distinct().CountAsync(cancellation);
        var comentarios = await notas.CountAsync(cancellation);
        return Ok(new { participantes, comentarios, actualizadoUtc = DateTimeOffset.UtcNow });
    }
}
