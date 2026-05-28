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

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<SolicitudAcademicaDto>> ObtenerPorId(Guid id)
    {
        var resultado = await mediator.Send(new ObtenerSolicitudPorIdQuery(id));
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/asignar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> AsignarAsesor(Guid id, [FromBody] Guid asesorId)
    {
        await mediator.Send(new AsignarAsesorCommand(id, asesorId));
        return NoContent();
    }

    [HttpPost("{id:guid}/postular")]
    [Authorize(Roles = "Asesor")]
    public async Task<ActionResult> Postular(Guid id, [FromBody] Guid colaboradorId)
    {
        await mediator.Send(new PostularASolicitudCommand(id, colaboradorId));
        return NoContent();
    }

    [HttpPost("{id:guid}/seleccionar-postulado")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> SeleccionarPostulado(Guid id, [FromBody] Guid colaboradorId)
    {
        await mediator.Send(new SeleccionarPostuladoCommand(id, colaboradorId));
        return NoContent();
    }

    [HttpGet("{id:guid}/avances")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<AvanceDto>>> ObtenerAvances(Guid id)
    {
        var resultado = await mediator.Send(new ObtenerAvancesPorSolicitudQuery(id));
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/avances")]
    [Authorize(Roles = "Asesor")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AvanceDto>> SubirAvance(Guid id,
        [FromForm] Guid solicitudId,
        [FromForm] Guid asesorId,
        [FromForm] string descripcion,
        [FromForm] int numeroFase,
        [FromForm] int tipo,
        [FromForm] List<IFormFile> archivos)
    {
        if (solicitudId != id)
            return BadRequest(new { mensaje = "El ID de la solicitud en la URL no coincide con el cuerpo." });

        var archivosInput = archivos.Select(a => new ArchivoAdjuntoInput(
            a.FileName,
            a.ContentType,
            a.Length,
            a.OpenReadStream())).ToList();

        var comando = new SubirAvanceCommand(solicitudId, asesorId, descripcion, numeroFase, tipo, archivosInput);

        var resultado = await mediator.Send(comando);
        return CreatedAtAction(nameof(ObtenerAvances), new { id }, resultado);
    }

    [HttpPost("avances/{avanceId:guid}/comentarios")]
    [Authorize]
    public async Task<ActionResult> AgregarComentario(Guid avanceId, AgregarComentarioCommand comando)
    {
        if (comando.AvanceId != avanceId)
            return BadRequest(new { mensaje = "El ID del avance en la URL no coincide con el cuerpo." });

        await mediator.Send(comando);
        return NoContent();
    }

    [HttpPost("avances/{avanceId:guid}/archivos")]
    [Authorize(Roles = "Asesor")]
    public async Task<ActionResult> AgregarArchivo(Guid avanceId, AgregarArchivoCommand comando)
    {
        if (comando.AvanceId != avanceId)
            return BadRequest(new { mensaje = "El ID del avance en la URL no coincide con el cuerpo." });

        await mediator.Send(comando);
        return NoContent();
    }

    [HttpPost("avances/{avanceId:guid}/aprobar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> AprobarAvance(Guid avanceId)
    {
        await mediator.Send(new AprobarAvanceCommand(avanceId));
        return NoContent();
    }

    [HttpPost("avances/{avanceId:guid}/rechazar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult> RechazarAvance(Guid avanceId)
    {
        await mediator.Send(new RechazarAvanceCommand(avanceId));
        return NoContent();
    }
}
