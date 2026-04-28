using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad.Events;

namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Raíz de Agregado del contexto delimitado IDENTIDAD.
/// Representa un usuario del sistema con sus credenciales y perfil básico.
/// </summary>
public sealed class Usuario : RaizAgregado
{
    /// <summary>
    /// Nombre de usuario único para el inicio de sesión.
    /// </summary>
    public string NombreUsuario { get; private set; }

    /// <summary>
    /// Credenciales de acceso (hash de la clave).
    /// </summary>
    public ClaveAcceso Clave { get; private set; }

    /// <summary>
    /// Nombre completo del usuario para mostrar en la UI.
    /// </summary>
    public string Nombre { get; private set; }

    /// <summary>
    /// Ruta o URL de la imagen de perfil del usuario (puede ser nula).
    /// </summary>
    public string? Imagen { get; private set; }

    /// <summary>
    /// Indica si la cuenta del usuario está activa.
    /// </summary>
    public bool EstaActivo { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) de creación del usuario.
    /// </summary>
    public DateTimeOffset FechaCreacion { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) del último inicio de sesión exitoso.
    /// </summary>
    public DateTimeOffset? UltimoInicioSesion { get; private set; }

    // Constructor privado para EF Core / rehidratación
#pragma warning disable CS8618 // Requerido por EF Core para rehidratación de entidades
    private Usuario() : base() { }
#pragma warning restore CS8618

    private Usuario(
        string nombreUsuario,
        ClaveAcceso clave,
        string nombre,
        string? imagen) : base()
    {
        ValidarNombreUsuario(nombreUsuario);
        ValidarNombre(nombre);

        NombreUsuario = nombreUsuario.Trim().ToLowerInvariant();
        Clave = clave;
        Nombre = nombre.Trim();
        Imagen = imagen;
        EstaActivo = true;
        FechaCreacion = DateTimeOffset.UtcNow;
        UltimoInicioSesion = null;

        AgregarEventoDominio(new UsuarioCreadoEvento(Id, NombreUsuario));
    }

    /// <summary>
    /// Crea un nuevo usuario del sistema.
    /// </summary>
    /// <param name="nombreUsuario">Nombre de usuario único (se normaliza a minúsculas).</param>
    /// <param name="hashClave">Hash de la clave generado por el servicio de hashing.</param>
    /// <param name="nombre">Nombre completo del usuario.</param>
    /// <param name="imagen">Ruta o URL de la imagen de perfil (opcional).</param>
    public static Usuario Crear(
        string nombreUsuario,
        string hashClave,
        string nombre,
        string? imagen = null)
    {
        var clave = ClaveAcceso.Crear(hashClave);
        return new Usuario(nombreUsuario, clave, nombre, imagen);
    }

    /// <summary>
    /// Registra un inicio de sesión exitoso actualizando la marca temporal.
    /// </summary>
    public void RegistrarInicioSesion()
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("No se puede iniciar sesión con una cuenta inactiva.");

        UltimoInicioSesion = DateTimeOffset.UtcNow;
        AgregarEventoDominio(new SesionIniciadaEvento(Id, NombreUsuario));
    }

    /// <summary>
    /// Actualiza la clave de acceso del usuario.
    /// </summary>
    /// <param name="nuevoHashClave">Nuevo hash de clave generado por el servicio de hashing.</param>
    public void CambiarClave(string nuevoHashClave)
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("No se puede cambiar la clave de una cuenta inactiva.");

        Clave = ClaveAcceso.Crear(nuevoHashClave);
    }

    /// <summary>
    /// Actualiza la información del perfil del usuario.
    /// </summary>
    public void ActualizarPerfil(string nombre, string? imagen)
    {
        ValidarNombre(nombre);
        Nombre = nombre.Trim();
        Imagen = imagen;
    }

    /// <summary>
    /// Desactiva la cuenta del usuario.
    /// </summary>
    public void Desactivar()
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("La cuenta ya se encuentra inactiva.");

        EstaActivo = false;
    }

    /// <summary>
    /// Reactiva la cuenta del usuario.
    /// </summary>
    public void Activar()
    {
        if (EstaActivo)
            throw new ExcepcionDominio("La cuenta ya se encuentra activa.");

        EstaActivo = true;
    }

    // ── Validaciones de Invariantes ──────────────────────────

    private static void ValidarNombreUsuario(string nombreUsuario)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario))
            throw new ExcepcionDominio("El nombre de usuario es obligatorio.");

        if (nombreUsuario.Trim().Length < 3)
            throw new ExcepcionDominio("El nombre de usuario debe tener al menos 3 caracteres.");

        if (nombreUsuario.Trim().Length > 50)
            throw new ExcepcionDominio("El nombre de usuario no puede exceder 50 caracteres.");
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre del usuario es obligatorio.");

        if (nombre.Trim().Length > 150)
            throw new ExcepcionDominio("El nombre no puede exceder 150 caracteres.");
    }
}

