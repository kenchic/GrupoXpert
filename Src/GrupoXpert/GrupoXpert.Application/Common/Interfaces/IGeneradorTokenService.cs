namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Servicio para generar tokens seguros de un solo uso.
/// La implementación usa <see cref="System.Security.Cryptography.RandomNumberGenerator"/>
/// o similar en la capa de Infraestructura.
/// </summary>
public interface IGeneradorTokenService
{
    /// <summary>
    /// Genera un token único y criptográficamente seguro (formato URL-safe base64 o GUID).
    /// </summary>
    string GenerarToken();
}
