using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Academia.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GrupoXpert.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SolicitudesAcademicasController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SolicitudAcademicaDto>> Crear(CrearSolicitudAcademicaCommand comando)
    {
        var resultado = await mediator.Send(comando);
        return Ok(resultado);
    }
}
