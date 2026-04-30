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
    private const string EmailValido          = "admin@grupoxpert.com";
    private const string HashClaveValida      = "hash_seguro_bcrypt_ejemplo";
    private const string NombreValido         = "Germán Álvarez";
    private const string TokenActivacion      = "token-uuid-v4-activacion-123";

    // ── Fábrica auxiliar ─────────────────────────────────────────────────────

    private static Usuario CrearUsuarioPorDefecto() =>
        Usuario.Crear(EmailValido, HashClaveValida, NombreValido, TokenActivacion);

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
        usuario.Email.Valor.Should().Be(EmailValido);
        usuario.Nombre.Should().Be(NombreValido);
        usuario.EstaActivo.Should().BeFalse("la cuenta debe iniciar inactiva hasta confirmar el correo");
        usuario.TokenActivacion.Should().Be(TokenActivacion);
        usuario.TokenActivacionExpira.Should().NotBeNull();
        usuario.UltimoInicioSesion.Should().BeNull();
    }

    [Fact]
    public void Crear_CuandoDatosValidos_DebeNormalizarEmailAMinusculas()
    {
        // Arrange
        const string emailConMayusculas = "Admin@GrupoXpert.COM";

        // Act
        var usuario = Usuario.Crear(emailConMayusculas, HashClaveValida, NombreValido, TokenActivacion);

        // Assert
        usuario.Email.Valor.Should().Be("admin@grupoxpert.com");
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
    public void Crear_CuandoEmailEsVacio_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => Usuario.Crear(string.Empty, HashClaveValida, NombreValido, TokenActivacion);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El correo electrónico es obligatorio.");
    }

    [Fact]
    public void Crear_CuandoEmailTieneFormatoInvalido_DebeLanzarExcepcionDominio()
    {
        // Arrange
        const string emailMalformado = "no-es-un-email";

        // Act
        var accion = () => Usuario.Crear(emailMalformado, HashClaveValida, NombreValido, TokenActivacion);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage($"El formato del correo electrónico '{emailMalformado}' no es válido.");
    }

    [Fact]
    public void Crear_CuandoNombreEsVacio_DebeLanzarExcepcionDominio()
    {
        // Act
        var accion = () => Usuario.Crear(EmailValido, HashClaveValida, string.Empty, TokenActivacion);

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
        var accion = () => Usuario.Crear(EmailValido, HashClaveValida, nombreMuyLargo, TokenActivacion);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("El nombre no puede exceder 150 caracteres.");
    }

    [Fact]
    public void Crear_CuandoImagenEsNula_DebeCrearseCorrectamente()
    {
        // Act
        var usuario = Usuario.Crear(EmailValido, HashClaveValida, NombreValido, TokenActivacion, imagen: null);

        // Assert
        usuario.Imagen.Should().BeNull();
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
}
