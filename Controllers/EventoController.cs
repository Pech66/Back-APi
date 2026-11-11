using Microsoft.AspNetCore.Mvc;
namespace LABOFMUTANTAPI.Controllers;
[ApiController]
[Route("api/evento")]
public sealed class EventoController(IConfiguration config) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new {
        titulo = config["Evento:Titulo"], fecha = config["Evento:Fecha"],
        hora = config["Evento:Hora"], lugar = config["Evento:Lugar"],
        direccion = config["Evento:Direccion"], descripcion = config["Evento:Descripcion"]
    });
}
