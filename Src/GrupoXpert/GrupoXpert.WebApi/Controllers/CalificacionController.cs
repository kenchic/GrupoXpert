using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Calificacion.Commands;
using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Calificacion.Queries;
using GrupoXpert.Domain.Identidad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrupoXpert.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CalificacionController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Estudiante")]
    public async Task<ActionResult<CalificacionColaboradorDto>> Calificar(CalificarColaboradorCommand comando)
    {
        var resultado = await mediator.Send(comando);
        return Ok(resultado);
    }

    [HttpGet("solicitudes-por-calificar")]
    [Authorize(Roles = "Estudiante")]
    public async Task<ActionResult<IReadOnlyList<SolicitudPorCalificarDto>>> ObtenerSolicitudesPorCalificar()
    {
        var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
            return Unauthorized();

        var resultado = await mediator.Send(new ObtenerSolicitudesPorCalificarQuery(usuarioId));
        return Ok(resultado);
    }

    [HttpGet("solicitud/{solicitudId:guid}")]
    [Authorize]
    public async Task<ActionResult<CalificacionColaboradorDto>> ObtenerPorSolicitud(Guid solicitudId)
    {
        var resultado = await mediator.Send(new ObtenerCalificacionPorSolicitudQuery(solicitudId));
        if (resultado is null) return NotFound();
        return Ok(resultado);
    }

    [HttpGet("colaborador/{colaboradorId:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IReadOnlyList<CalificacionColaboradorDto>>> ObtenerPorColaborador(Guid colaboradorId)
    {
        var resultado = await mediator.Send(new ObtenerCalificacionesPorColaboradorQuery(colaboradorId));
        return Ok(resultado);
    }

    [HttpGet("mis-calificaciones")]
    [Authorize(Roles = "Asesor")]
    public async Task<ActionResult<IReadOnlyList<CalificacionColaboradorDto>>> ObtenerMisCalificaciones()
    {
        var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
            return Unauthorized();

        var resultado = await mediator.Send(new ObtenerMisCalificacionesQuery(usuarioId));
        return Ok(resultado);
    }

    [HttpGet("todas")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IReadOnlyList<CalificacionColaboradorDto>>> ObtenerTodas()
    {
        var resultado = await mediator.Send(new ObtenerTodasCalificacionesQuery());
        return Ok(resultado);
    }

    [HttpPost("solicitud/{solicitudId:guid}/liberar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> LiberarSolicitud(Guid solicitudId)
    {
        await mediator.Send(new LiberarSolicitudCommand(solicitudId));
        return NoContent();
    }
}