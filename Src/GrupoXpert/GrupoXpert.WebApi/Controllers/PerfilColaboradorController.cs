using System;
using System.Threading.Tasks;
using GrupoXpert.Application.Perfil.Commands;
using GrupoXpert.Application.Perfil.Dtos;
using GrupoXpert.Application.Perfil.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrupoXpert.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerfilColaboradorController(IMediator mediator) : ControllerBase
{
    [HttpGet("{usuarioId:guid}")]
    public async Task<ActionResult<PerfilColaboradorDto>> ObtenerPerfil(Guid usuarioId)
    {
        var query = new ObtenerPerfilColaboradorQuery(usuarioId);
        var resultado = await mediator.Send(query);

        if (resultado == null) return NotFound("Perfil de colaborador no encontrado.");

        return Ok(resultado);
    }

    [HttpPut]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilColaboradorCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPost("validar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ValidarPerfil([FromBody] ValidarPerfilProfesionalCommand command)
    {
        try
        {
            await mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
