using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Application.Academia.Queries;
using GrupoXpert.Domain.Identidad;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("dashboard")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<SolicitudAcademicaDto>>> ObtenerDashboard()
    {
        var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var rolStr = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(usuarioIdStr, out var usuarioId) || !Enum.TryParse<TipoUsuario>(rolStr, out var rol))
        {
            return Unauthorized();
        }

        var resultado = await mediator.Send(new ObtenerSolicitudesDashboardQuery(usuarioId, rol));
        return Ok(resultado);
    }
}
