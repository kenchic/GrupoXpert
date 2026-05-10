using FluentAssertions;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Identidad.Events;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Identidad;

/// <summary>
/// Pruebas de dominio para el Agregado Usuario.
/// Validan invariantes y reglas de negocio puras SIN dependencias de infraestructura.
/// </summary>
public sealed class UsuarioTests
{
    // ── Constantes de prueba ─────────────────────────────────────────────────
    private const string CorreoValido         = "admin@grupoxpert.com";
    private const string HashClaveValida      = "hash_seguro_bcrypt_ejemplo";
    private const string NombreValido         = "Germán Álvarez";
    private const string TokenActivacion      = "token-uuid-v4-activacion-123";

    // ── Fábrica auxiliar ─────────────────────────────────────────────────────

    private static Usuario CrearUsuarioPorDefecto() =>
        Usuario.Crear(CorreoValido, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Estudiante);

    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN DEL AGREGADO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Crear_CuandoDatosValidos_DebeCrearUsuarioInactivo()
    {
        // Act
        var usuario = CrearUsuarioPorDefecto();

        // Assert
        usuario.Should().NotBeNull();
        usuario.Id.Should().NotBeEmpty();
        usuario.Correo.Valor.Should().Be(CorreoValido);
        usuario.Nombre.Should().Be(NombreValido);
        usuario.EstaActivo.Should().BeFalse("la cuenta debe iniciar inactiva hasta confirmar el correo");
        usuario.TokenActivacion.Should().Be(TokenActivacion);
        usuario.TokenActivacionExpira.Should().NotBeNull();
        usuario.UltimoInicioSesion.Should().BeNull();
    }

    [Fact]
    public void Crear_CuandoDatosValidos_DebeNormalizarCorreoAMinusculas()
    {
        // Arrange
        const string correoConMayusculas = "Admin@GrupoXpert.COM";

        // Act
        var usuario = Usuario.Crear(correoConMayusculas, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        usuario.Correo.Valor.Should().Be("admin@grupoxpert.com");
    }

    [Fact]
    public void Crear_CuandoDatosValidos_DebeEmitirEventoUsuarioCreado()
    {
        // Act
        var usuario = CrearUsuarioPorDefecto();

        // Assert
        usuario.EventosDominio
            .Should().ContainSingle(e => e is UsuarioCreadoEvent,
                "se debe emitir exactamente un evento UsuarioCreadoEvent al crear un usuario");
    }

    [Fact]
    public void Crear_CuandoCorreoEsVacio_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => Usuario.Crear(string.Empty, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El correo electrónico es obligatorio.");
    }

    [Fact]
    public void Crear_CuandoCorreoTieneFormatoInvalido_DebeLanzarExcepcionDominio()
    {
        // Arrange
        const string correoMalformado = "no-es-un-email";

        // Act
        var accion = () => Usuario.Crear(correoMalformado, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage($"El formato del correo electrónico '{correoMalformado}' no es válido.");
    }

    [Fact]
    public void Crear_CuandoNombreEsVacio_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => Usuario.Crear(CorreoValido, HashClaveValida, string.Empty, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El nombre del usuario es obligatorio.");
    }

    [Fact]
    public void Crear_CuandoNombreExcede150Caracteres_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var nombreMuyLargo = new string('A', 151);

        // Act
        var accion = () => Usuario.Crear(CorreoValido, HashClaveValida, nombreMuyLargo, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El nombre no puede exceder 150 caracteres.");
    }

    [Fact]
    public void Crear_CuandoImagenEsNula_DebeCrearseCorrectamente()
    {
        // Act
        var usuario = Usuario.Crear(CorreoValido, HashClaveValida, NombreValido, TokenActivacion, imagen: null, tipo: TipoUsuario.Estudiante);

        // Assert
        usuario.Imagen.Should().BeNull();
    }

    [Fact]
    public void Crear_CuandoTipoEsEstudiante_DebeAsignarTipoCorrectamente()
    {
        // Act
        var usuario = Usuario.Crear(CorreoValido, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Estudiante);

        // Assert
        usuario.Tipo.Should().Be(TipoUsuario.Estudiante);
    }

    [Fact]
    public void Crear_CuandoTipoEsAsesor_DebeAsignarTipoCorrectamente()
    {
        // Act
        var usuario = Usuario.Crear(CorreoValido, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Asesor);

        // Assert
        usuario.Tipo.Should().Be(TipoUsuario.Asesor);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ACTIVACIÓN DE CUENTA
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void ActivarCuenta_CuandoTokenEsValido_DebeActivarLaCuenta()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();

        // Act
        usuario.ActivarCuenta(TokenActivacion);

        // Assert
        usuario.EstaActivo.Should().BeTrue();
        usuario.TokenActivacion.Should().BeNull("el token debe invalidarse tras la activación");
        usuario.TokenActivacionExpira.Should().BeNull("la fecha de expiración debe limpiarse tras la activación");
    }

    [Fact]
    public void ActivarCuenta_CuandoTokenEsValido_DebeEmitirEventoCuentaActivada()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();

        // Act
        usuario.ActivarCuenta(TokenActivacion);

        // Assert
        usuario.EventosDominio
            .Should().Contain(e => e is CuentaActivadaEvent,
                "se debe emitir un evento CuentaActivadaEvent al activar la cuenta");
    }

    [Fact]
    public void ActivarCuenta_CuandoTokenEsIncorrecto_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();

        // Act
        var accion = () => usuario.ActivarCuenta("token-incorrecto-xyz");

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El enlace de activación no es válido.");
    }

    [Fact]
    public void ActivarCuenta_CuandoCuentaYaEstaActiva_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();
        usuario.ActivarCuenta(TokenActivacion);

        // Act — intentar activar de nuevo
        var accion = () => usuario.ActivarCuenta(TokenActivacion);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("La cuenta ya se encuentra activa.");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // OPERACIONES POST-CREACIÓN
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void CambiarClave_CuandoCuentaEstaActiva_DebeActualizarElHash()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();
        usuario.ActivarCuenta(TokenActivacion);
        const string nuevoHash = "nuevo_hash_bcrypt_2026";

        // Act
        usuario.CambiarClave(nuevoHash);

        // Assert
        usuario.Clave.HashClave.Should().Be(nuevoHash);
    }

    [Fact]
    public void CambiarClave_CuandoCuentaEstaInactiva_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();

        // Act
        var accion = () => usuario.CambiarClave("nuevo_hash");

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("No se puede cambiar la clave de una cuenta inactiva.");
    }

    [Fact]
    public void Desactivar_CuandoCuentaEstaActiva_DebeDesactivarla()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();
        usuario.ActivarCuenta(TokenActivacion);

        // Act
        usuario.Desactivar();

        // Assert
        usuario.EstaActivo.Should().BeFalse();
    }

    [Fact]
    public void Desactivar_CuandoCuentaYaEstaInactiva_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto(); // inactiva por defecto

        // Act
        var accion = () => usuario.Desactivar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("La cuenta ya se encuentra inactiva.");
    }

    [Fact]
    public void ActualizarPerfil_CuandoDatosValidos_DebeActualizarNombreEImagen()
    {
        // Arrange
        var usuario = CrearUsuarioPorDefecto();
        const string nuevoNombre = "Nuevo Nombre Usuario";
        const string nuevaImagen = "https://cdn.grupoxpert.com/perfiles/nuevo.png";

        // Act
        usuario.ActualizarPerfil(nuevoNombre, nuevaImagen);

        // Assert
        usuario.Nombre.Should().Be(nuevoNombre);
        usuario.Imagen.Should().Be(nuevaImagen);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // APROBACIÓN DE COLABORADOR (CU3)
    // ═══════════════════════════════════════════════════════════════════════════

    private static Usuario CrearAsesorPorDefecto() =>
        Usuario.Crear(CorreoValido, HashClaveValida, NombreValido, TokenActivacion, tipo: TipoUsuario.Asesor);

    [Fact]
    public void Crear_CuandoTipoEsEstudiante_DebeAutoAprobarse()
    {
        // Act
        var usuario = CrearUsuarioPorDefecto();

        // Assert
        usuario.EstaAprobado.Should().BeTrue("los estudiantes no requieren aprobación de administrador");
    }

    [Fact]
    public void Crear_CuandoTipoEsAsesor_NoDebeEstarAprobado()
    {
        // Act
        var usuario = CrearAsesorPorDefecto();

        // Assert
        usuario.EstaAprobado.Should().BeFalse("los asesores requieren aprobación explícita de un administrador");
    }

    [Fact]
    public void AprobarCuenta_CuandoEsAsesorPendiente_DebeAprobarYActivar()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        var administradorId = Guid.NewGuid();

        // Act
        asesor.AprobarCuenta(administradorId);

        // Assert
        asesor.EstaAprobado.Should().BeTrue();
        asesor.EstaActivo.Should().BeTrue("la aprobación también activa la cuenta");
        asesor.FechaAprobacion.Should().NotBeNull();
        asesor.AprobadoPorId.Should().Be(administradorId);
        asesor.TokenActivacion.Should().BeNull("el token se invalida al aprobar");
    }

    [Fact]
    public void AprobarCuenta_CuandoEsAsesor_DebeEmitirEventoColaboradorAprobado()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        var administradorId = Guid.NewGuid();

        // Act
        asesor.AprobarCuenta(administradorId);

        // Assert
        asesor.EventosDominio
            .Should().Contain(e => e is ColaboradorAprobadoEvent,
                "se debe emitir un evento ColaboradorAprobadoEvent al aprobar");
    }

    [Fact]
    public void AprobarCuenta_CuandoYaEstaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        asesor.AprobarCuenta(Guid.NewGuid());

        // Act
        var accion = () => asesor.AprobarCuenta(Guid.NewGuid());

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("La cuenta del colaborador ya fue aprobada.");
    }

    [Fact]
    public void AprobarCuenta_CuandoEsEstudiante_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var estudiante = CrearUsuarioPorDefecto();

        // Act
        var accion = () => estudiante.AprobarCuenta(Guid.NewGuid());

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("Solo las cuentas de tipo Asesor requieren aprobación.");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // REVOCACIÓN DE APROBACIÓN (CU3)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void RevocarAprobacion_CuandoEstaAprobado_DebeDesaprobarYDesactivar()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        asesor.AprobarCuenta(Guid.NewGuid());

        // Act
        asesor.RevocarAprobacion();

        // Assert
        asesor.EstaAprobado.Should().BeFalse();
        asesor.EstaActivo.Should().BeFalse("la revocación desactiva la cuenta");
        asesor.FechaAprobacion.Should().BeNull();
        asesor.AprobadoPorId.Should().BeNull();
    }

    [Fact]
    public void RevocarAprobacion_CuandoNoEstaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();

        // Act
        var accion = () => asesor.RevocarAprobacion();

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("La cuenta del colaborador no está aprobada.");
    }

    [Fact]
    public void RevocarAprobacion_CuandoEsEstudiante_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var estudiante = CrearUsuarioPorDefecto();

        // Act
        var accion = () => estudiante.RevocarAprobacion();

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("Solo las cuentas de tipo Asesor pueden revocarse.");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // INICIO DE SESIÓN CON VALIDACIÓN DE APROBACIÓN (CU3)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void RegistrarInicioSesion_CuandoAsesorNoAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        asesor.ActivarCuenta(TokenActivacion); // activar pero NO aprobar

        // Act
        var accion = () => asesor.RegistrarInicioSesion();

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("Tu cuenta está pendiente de aprobación por un administrador.");
    }

    [Fact]
    public void RegistrarInicioSesion_CuandoAsesorAprobado_DebeRegistrarSesion()
    {
        // Arrange
        var asesor = CrearAsesorPorDefecto();
        asesor.AprobarCuenta(Guid.NewGuid()); // aprueba Y activa

        // Act
        asesor.RegistrarInicioSesion();

        // Assert
        asesor.UltimoInicioSesion.Should().NotBeNull();
    }
}
