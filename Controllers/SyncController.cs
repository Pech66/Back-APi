using LABOFMUTANTAPI.Dto;
using LABOFMUTANTAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LABOFMUTANTAPI.Controllers;
[ApiController]
[Route("api/sync")]
public sealed class SyncController(ISyncService service) : ControllerBase
{
    private string? DeviceToken()
    {
        var authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;
        var token = authorization[7..];
        return Guid.TryParseExact(token, "D", out _) ? token.ToLowerInvariant() : null;
    }
    [HttpPost]
    [RequestSizeLimit(262144)]
    public async Task<IActionResult> Post([FromBody] SyncRequestDto request, CancellationToken cancellation)
    {
        var token = DeviceToken();
        if (token is null) return Unauthorized(new { message = "Falta el identificador secreto del dispositivo." });
        try { return Ok(await service.Receive(token, request, cancellation)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        var token = DeviceToken();
        if (token is null) return Unauthorized();
        return Ok(await service.Get(token, cancellation));
    }
}
