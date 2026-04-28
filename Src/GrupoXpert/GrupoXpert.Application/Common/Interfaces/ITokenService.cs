using GrupoXpert.Domain.Identidad;

namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Define los métodos para la generación de tokens de seguridad (JWT).
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token de acceso para un usuario autenticado.
    /// </summary>
    string GenerarToken(Usuario usuario);
}

