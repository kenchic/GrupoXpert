using System.Text.RegularExpressions;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Objeto de Valor que encapsula el correo electrónico del usuario.
/// Se normaliza a minúsculas y se valida con un patrón estándar RFC 5322 simplificado.
/// </summary>
public sealed class CorreoElectronico : ValueObject
{
    private static readonly Regex PatronEmail =
        new(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Dirección de correo electrónico normalizada (siempre en minúsculas).
    /// </summary>
    public string Valor { get; }

    private CorreoElectronico(string valor)
    {
        Valor = valor;
    }

    /// <summary>
    /// Crea un nuevo CorreoElectronico validando el formato y normalizando a minúsculas.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico a validar.</param>
    /// <exception cref="ExcepcionDominio">Si el formato es inválido o está vacío.</exception>
    public static CorreoElectronico Crear(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ExcepcionDominio("El correo electrónico es obligatorio.");

        var normalizado = email.Trim().ToLowerInvariant();

        if (normalizado.Length > 254)
            throw new ExcepcionDominio("El correo electrónico no puede exceder 254 caracteres.");

        if (!PatronEmail.IsMatch(normalizado))
            throw new ExcepcionDominio($"El formato del correo electrónico '{email}' no es válido.");

        return new CorreoElectronico(normalizado);
    }

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}
