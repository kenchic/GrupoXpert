using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad.Events;

namespace GrupoXpert.Domain.Identidad;

/// <summary>
/// Raíz de Agregado del contexto delimitado IDENTIDAD.
/// Representa un usuario del sistema que se identifica por su correo electrónico único.
/// La cuenta inicia inactiva y se activa mediante un enlace enviado al correo.
/// </summary>
public sealed class Usuario : AggregateRoot
{
    /// <summary>
    /// Correo electrónico único del usuario (identificador principal de login).
    /// </summary>
    public CorreoElectronico Email { get; private set; }

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
    /// Indica si la cuenta del usuario ha sido activada mediante el enlace de correo.
    /// Las cuentas nuevas inician con valor <c>false</c>.
    /// </summary>
    public bool EstaActivo { get; private set; }

    /// <summary>
    /// Token único para activar la cuenta. Se invalida al completar la activación.
    /// </summary>
    public string? TokenActivacion { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) de expiración del token de activación.
    /// </summary>
    public DateTimeOffset? TokenActivacionExpira { get; private set; }

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
        CorreoElectronico email,
        ClaveAcceso clave,
        string nombre,
        string? imagen,
        string tokenActivacion) : base()
    {
        ValidarNombre(nombre);

        Email = email;
        Clave = clave;
        Nombre = nombre.Trim();
        Imagen = imagen;
        EstaActivo = false; // La cuenta inicia inactiva hasta confirmar el email
        TokenActivacion = tokenActivacion;
        TokenActivacionExpira = DateTimeOffset.UtcNow.AddHours(24);
        FechaCreacion = DateTimeOffset.UtcNow;
        UltimoInicioSesion = null;

        AgregarEventoDominio(new UsuarioCreadoEvent(Id, email.Valor, tokenActivacion));
    }

    /// <summary>
    /// Crea un nuevo usuario del sistema con la cuenta pendiente de activación.
    /// </summary>
    /// <param name="email">Correo electrónico único (se normaliza a minúsculas).</param>
    /// <param name="hashClave">Hash de la clave generado en la capa de Infraestructura.</param>
    /// <param name="nombre">Nombre completo del usuario.</param>
    /// <param name="tokenActivacion">Token único generado para el enlace de activación.</param>
    /// <param name="imagen">Ruta o URL de la imagen de perfil (opcional).</param>
    public static Usuario Crear(
        string email,
        string hashClave,
        string nombre,
        string tokenActivacion,
        string? imagen = null)
    {
        var correo = CorreoElectronico.Crear(email);
        var clave = ClaveAcceso.Crear(hashClave);
        return new Usuario(correo, clave, nombre, imagen, tokenActivacion);
    }

    /// <summary>
    /// Activa la cuenta del usuario validando el token y su vigencia.
    /// </summary>
    /// <param name="token">Token de activación recibido en el enlace de correo.</param>
    /// <exception cref="ExcepcionDominio">Si el token es inválido, ya fue usado o está expirado.</exception>
    public void ActivarCuenta(string token)
    {
        if (EstaActivo)
            throw new ExcepcionDominio("La cuenta ya se encuentra activa.");

        if (string.IsNullOrWhiteSpace(TokenActivacion) || TokenActivacion != token)
            throw new ExcepcionDominio("El enlace de activación no es válido.");

        if (DateTimeOffset.UtcNow > TokenActivacionExpira)
            throw new ExcepcionDominio("El enlace de activación ha expirado. Solicite uno nuevo.");

        EstaActivo = true;
        TokenActivacion = null;       // Invalidar el token tras usarlo
        TokenActivacionExpira = null;

        AgregarEventoDominio(new CuentaActivadaEvent(Id, Email.Valor));
    }

    /// <summary>
    /// Registra un inicio de sesión exitoso actualizando la marca temporal.
    /// </summary>
    public void RegistrarInicioSesion()
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("La cuenta no está activa. Por favor verifica tu correo para activarla.");

        UltimoInicioSesion = DateTimeOffset.UtcNow;
        AgregarEventoDominio(new SesionIniciadaEvent(Id, Email.Valor));
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
    /// Desactiva la cuenta del usuario (acción administrativa).
    /// </summary>
    public void Desactivar()
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("La cuenta ya se encuentra inactiva.");

        EstaActivo = false;
    }

    // ── Validaciones de Invariantes ──────────────────────────

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre del usuario es obligatorio.");

        if (nombre.Trim().Length > 150)
            throw new ExcepcionDominio("El nombre no puede exceder 150 caracteres.");
    }
}
