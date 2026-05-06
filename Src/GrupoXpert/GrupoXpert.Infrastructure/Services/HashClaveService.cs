using GrupoXpert.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace GrupoXpert.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de hashing usando BCrypt.
/// </summary>
public sealed class HashClaveService : IHashClaveService
{
    public string GenerarHash(string clave)
    {
        return BC.HashPassword(clave);
    }

    public bool Verificar(string clave, string hash)
    {
        return BC.Verify(clave, hash);
    }
}

