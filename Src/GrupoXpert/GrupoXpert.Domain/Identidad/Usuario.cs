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
    public CorreoElectronico Correo { get; private set; }

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
    /// Tipo de usuario en la plataforma (Estudiante o Asesor).
    /// </summary>
    public TipoUsuario Tipo { get; private set; }

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

    /// <summary>
    /// Indica si la cuenta del colaborador fue aprobada por un administrador.
    /// Solo aplica para usuarios de tipo Asesor.
    /// </summary>
    public bool EstaAprobado { get; private set; }

    /// <summary>
    /// Fecha y hora (UTC) en la que un administrador aprobó la cuenta del colaborador.
    /// </summary>
    public DateTimeOffset? FechaAprobacion { get; private set; }

    /// <summary>
    /// Identificador del administrador que aprobó la cuenta.
    /// </summary>
    public Guid? AprobadoPorId { get; private set; }

    /// <summary>
    /// Estado actual de verificación del perfil del colaborador.
    /// </summary>
    public EstadoVerificacion EstadoVerificacion { get; private set; }

    // Constructor privado para EF Core / rehidratación
#pragma warning disable CS8618 // Requerido por EF Core para rehidratación de entidades
    private Usuario() : base() { }
#pragma warning restore CS8618

    private Usuario(
        CorreoElectronico correo,
        ClaveAcceso clave,
        string nombre,
        string? imagen,
        TipoUsuario tipo,
        string tokenActivacion) : base()
    {
        ValidarNombre(nombre);
        ValidarTipo(tipo);

        Correo = correo;
        Clave = clave;
        Nombre = nombre.Trim();
        Imagen = imagen;
        Tipo = tipo;
        EstaActivo = false;
        EstaAprobado = tipo == TipoUsuario.Estudiante || tipo == TipoUsuario.Administrador || tipo == TipoUsuario.Revisor;
        EstadoVerificacion = tipo == TipoUsuario.Asesor ? EstadoVerificacion.Pendiente : EstadoVerificacion.Aprobado;
        TokenActivacion = tokenActivacion;
        TokenActivacionExpira = DateTimeOffset.UtcNow.AddHours(24);
        FechaCreacion = DateTimeOffset.UtcNow;
        UltimoInicioSesion = null;

        AgregarEventoDominio(new UsuarioCreadoEvent(Id, correo.Valor, tokenActivacion));
    }

    /// <summary>
    /// Crea un nuevo usuario del sistema con la cuenta pendiente de activación.
    /// </summary>
    /// <param name="correo">Correo electrónico único (se normaliza a minúsculas).</param>
    /// <param name="hashClave">Hash de la clave generado en la capa de Infraestructura.</param>
    /// <param name="nombre">Nombre completo del usuario.</param>
    /// <param name="tokenActivacion">Token único generado para el enlace de activación.</param>
    /// <param name="imagen">Ruta o URL de la imagen de perfil (opcional).</param>
    /// <param name="tipo">Tipo de usuario (Estudiante o Asesor).</param>
    public static Usuario Crear(
        string correo,
        string hashClave,
        string nombre,
        string tokenActivacion,
        string? imagen = null,
        TipoUsuario tipo = TipoUsuario.Estudiante)
    {
        var correoElectronico = CorreoElectronico.Crear(correo);
        var clave = ClaveAcceso.Crear(hashClave);
        return new Usuario(correoElectronico, clave, nombre, imagen, tipo, tokenActivacion);
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

        AgregarEventoDominio(new CuentaActivadaEvent(Id, Correo.Valor));
    }

    /// <summary>
    /// Registra un inicio de sesión exitoso actualizando la marca temporal.
    /// </summary>
    public void RegistrarInicioSesion()
    {
        if (!EstaActivo)
            throw new ExcepcionDominio("La cuenta no está activa. Por favor verifica tu correo para activarla.");

        if (Tipo == TipoUsuario.Asesor && !EstaAprobado)
            throw new ExcepcionDominio("Tu cuenta está pendiente de aprobación por un administrador.");

        UltimoInicioSesion = DateTimeOffset.UtcNow;
        AgregarEventoDominio(new SesionIniciadaEvent(Id, Correo.Valor));
    }

    /// <summary>
    /// Aprueba la cuenta de un colaborador (solo Asesores). Acción exclusiva de un Administrador.
    /// </summary>
    public void AprobarCuenta(Guid administradorId)
    {
        if (Tipo != TipoUsuario.Asesor)
            throw new ExcepcionDominio("Solo las cuentas de tipo Asesor requieren aprobación.");

        if (EstaAprobado)
            throw new ExcepcionDominio("La cuenta del colaborador ya fue aprobada.");

        EstaAprobado = true;
        EstaActivo = true;
        FechaAprobacion = DateTimeOffset.UtcNow;
        AprobadoPorId = administradorId;
        TokenActivacion = null;
        TokenActivacionExpira = null;

        AgregarEventoDominio(new ColaboradorAprobadoEvent(Id, Correo.Valor, administradorId));
    }

    /// <summary>
    /// Valida el perfil del colaborador cambiando su estado a EnRevision o Aprobado según corresponda.
    /// </summary>
    public void ValidarPerfil()
    {
        if (Tipo != TipoUsuario.Asesor)
            throw new ExcepcionDominio("Solo las cuentas de tipo Asesor requieren validación de perfil.");

        if (EstadoVerificacion == EstadoVerificacion.Aprobado)
            throw new ExcepcionDominio("El perfil del colaborador ya fue aprobado.");

        EstadoVerificacion = EstadoVerificacion.EnRevision;
    }

    /// <summary>
    /// Rechaza/Desactiva la cuenta de un colaborador aprobado previamente.
    /// </summary>
    public void RevocarAprobacion()
    {
        if (Tipo != TipoUsuario.Asesor)
            throw new ExcepcionDominio("Solo las cuentas de tipo Asesor pueden revocarse.");

        if (!EstaAprobado)
            throw new ExcepcionDominio("La cuenta del colaborador no está aprobada.");

        EstaAprobado = false;
        EstaActivo = false;
        FechaAprobacion = null;
        AprobadoPorId = null;
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

    private static void ValidarTipo(TipoUsuario tipo)
    {
        if (!Enum.IsDefined(typeof(TipoUsuario), tipo))
            throw new ExcepcionDominio("El tipo de usuario no es válido.");
    }
}
