using System.Security.Cryptography;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Infrastructure.Services;

/// <summary>
/// Implementación del servicio para generar tokens seguros de un solo uso.
/// Utiliza RandomNumberGenerator para asegurar la entropía del token.
/// </summary>
public sealed class GeneradorTokenService : IGeneradorTokenService
{
    public string GenerarToken()
    {
        // Generamos 32 bytes de aleatoriedad y los convertimos a string URL-safe
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
