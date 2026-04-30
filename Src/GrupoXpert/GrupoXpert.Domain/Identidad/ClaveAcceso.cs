using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Objeto de Valor que encapsula las credenciales de acceso del usuario.
/// Contiene el hash de la clave, nunca la clave en texto plano.
/// </summary>
public sealed class ClaveAcceso : ValueObject
{
    /// <summary>
    /// Hash de la clave (generado con BCrypt, Argon2 o similar en la capa de Infraestructura).
    /// </summary>
    public string HashClave { get; }

    private ClaveAcceso(string hashClave)
    {
        if (string.IsNullOrWhiteSpace(hashClave))
            throw new ArgumentException("El hash de la clave no puede estar vacío.", nameof(hashClave));

        HashClave = hashClave;
    }

    /// <summary>
    /// Crea una nueva instancia de ClaveAcceso a partir de un hash ya generado.
    /// La generación del hash se delega a la capa de Infraestructura (servicio de hashing).
    /// </summary>
    public static ClaveAcceso Crear(string hashClave) => new(hashClave);

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return HashClave;
    }
}
