using GrupoXpert.Application.Calidad.Commands;
using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Calidad.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrupoXpert.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Revisor")]
public sealed class CalidadController(IMediator mediator) : ControllerBase
{
    [HttpPost("revisiones")]
    public async Task<ActionResult<RevisionCalidadDto>> CrearRevision(CrearRevisionCalidadCommand comando)
    {
        var resultado = await mediator.Send(comando);
        return Ok(resultado);
    }

    [HttpPost("revisiones/{id:guid}/visto-bueno")]
    public async Task<ActionResult<RevisionCalidadDto>> OtorgarVistoBueno(Guid id)
    {
        var resultado = await mediator.Send(new OtorgarVistoBuenoCommand(id));
        return Ok(resultado);
    }

    [HttpPost("revisiones/{id:guid}/rechazar")]
    public async Task<ActionResult<RevisionCalidadDto>> RechazarRevision(Guid id, [FromBody] RechazarRevisionCommand comando)
    {
        if (comando.RevisionId != id)
            return BadRequest(new { mensaje = "El ID de la revisión en la URL no coincide con el cuerpo." });

        var resultado = await mediator.Send(comando);
        return Ok(resultado);
    }

    [HttpGet("revisiones/pendientes")]
    public async Task<ActionResult<IReadOnlyList<RevisionCalidadDto>>> ObtenerPendientes()
    {
        var resultado = await mediator.Send(new ObtenerRevisionesPendientesQuery());
        return Ok(resultado);
    }

    [HttpGet("revisiones/avance/{avanceId:guid}")]
    public async Task<ActionResult<RevisionCalidadDto>> ObtenerPorAvance(Guid avanceId)
    {
        var resultado = await mediator.Send(new ObtenerRevisionPorAvanceQuery(avanceId));
        if (resultado is null) return NotFound();
        return Ok(resultado);
    }
}
