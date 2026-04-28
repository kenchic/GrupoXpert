namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Define los métodos para el hashing y verificación de contraseñas.
/// </summary>
public interface IHashClaveService
{
    /// <summary>
    /// Genera un hash seguro a partir de una clave en texto plano.
    /// </summary>
    string GenerarHash(string clave);

    /// <summary>
    /// Verifica si una clave en texto plano coincide con un hash almacenado.
    /// </summary>
    bool Verificar(string clave, string hash);
}

