using GrupoXpert.Application.Identidad.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrupoXpert.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de la autenticación de usuarios.
/// </summary>
[ApiController]
[Route("api/autenticacion")]
public sealed class AutenticacionController(ISender mediador) : ControllerBase
{
    private readonly ISender _mediador = mediador;

    /// <summary>
    /// Inicia sesión de un usuario y devuelve un token JWT.
    /// </summary>
    [HttpPost("iniciar-sesion")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IniciarSesion(
        [FromBody] IniciarSesionComando comando, 
        CancellationToken cancelacion)
    {
        try
        {
            var resultado = await _mediador.Send(comando, cancelacion);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            // En un entorno real, usar un Middleware de excepciones global
            // Aquí devolvemos 401 por simplicidad en el diseño inicial de login
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
}

