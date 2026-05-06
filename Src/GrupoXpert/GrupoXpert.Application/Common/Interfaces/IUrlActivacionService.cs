namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Servicio para construir URLs de activación de cuenta.
/// La implementación obtiene la URL base de la configuración de Infraestructura.
/// </summary>
public interface IUrlActivacionService
{
    /// <summary>
    /// Construye la URL completa de activación de cuenta a partir del token.
    /// Ejemplo: <c>https://app.grupoxpert.com/activar?token=ABC123</c>
    /// </summary>
    /// <param name="token">Token de activación único del usuario.</param>
    string Construir(string token);
}
