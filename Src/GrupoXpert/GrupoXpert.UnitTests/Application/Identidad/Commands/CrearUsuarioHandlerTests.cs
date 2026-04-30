using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Identidad.Commands;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Identidad.Commands;

/// <summary>
/// Pruebas del handler CrearUsuarioHandler (capa de Aplicación).
/// Validan la orquestación del flujo: unicidad de email, hashing, persistencia y envío de correo.
/// Se usan Mocks (NSubstitute) para todas las dependencias de infraestructura.
/// </summary>
public sealed class CrearUsuarioHandlerTests
{
    // ── Mocks de dependencias ────────────────────────────────────────────────
    private readonly IUsuarioRepository    _repositorioMock;
    private readonly IHashClaveService     _servicioHashMock;
    private readonly ICorreoElectronicoService _servicioCorreoMock;
    private readonly IGeneradorTokenService _generadorTokenMock;
    private readonly IUrlActivacionService  _urlActivacionMock;
    private readonly IUnidadDeTrabajo      _unidadDeTrabajoMock;
    private readonly CrearUsuarioHandler   _manejador;

    // ── Constantes de prueba ─────────────────────────────────────────────────
    private const string EmailValido      = "nuevo@grupoxpert.com";
    private const string ClaveTextoPlano  = "MiClave.Segura.2026!";
    private const string HashGenerado     = "hash_bcrypt_simulado_$2a$11$...";
    private const string TokenGenerado    = "d4f9a1b2-activacion-uuid-generado";
    private const string EnlaceActivacion = "https://app.grupoxpert.com/activar?token=d4f9a1b2-activacion-uuid-generado";
    private const string NombreUsuario    = "Nuevo Usuario";

    public CrearUsuarioHandlerTests()
    {
        _repositorioMock     = Substitute.For<IUsuarioRepository>();
        _servicioHashMock    = Substitute.For<IHashClaveService>();
        _servicioCorreoMock  = Substitute.For<ICorreoElectronicoService>();
        _generadorTokenMock  = Substitute.For<IGeneradorTokenService>();
        _urlActivacionMock   = Substitute.For<IUrlActivacionService>();
        _unidadDeTrabajoMock = Substitute.For<IUnidadDeTrabajo>();

        _manejador = new CrearUsuarioHandler(
            _repositorioMock,
            _servicioHashMock,
            _servicioCorreoMock,
            _generadorTokenMock,
            _urlActivacionMock,
            _unidadDeTrabajoMock);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // FLUJO EXITOSO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task RegistrarUsuario_CuandoDatosSonValidos_DebeRetornarIdDelUsuario()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        ConfigurarMocksParaFlujoExitoso();

        // Act
        var usuarioId = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        usuarioId.Should().NotBeEmpty("el handler debe retornar el ID generado para el nuevo usuario");
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoDatosSonValidos_DebeHashearLaClave()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        ConfigurarMocksParaFlujoExitoso();

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        _servicioHashMock.Received(1).GenerarHash(ClaveTextoPlano);
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoDatosSonValidos_DebeGenerarTokenDeActivacion()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        ConfigurarMocksParaFlujoExitoso();

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        _generadorTokenMock.Received(1).GenerarToken();
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoDatosSonValidos_DebePersistirElUsuario()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        ConfigurarMocksParaFlujoExitoso();

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await _repositorioMock.Received(1)
            .AgregarAsync(Arg.Any<Usuario>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajoMock.Received(1)
            .GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoDatosSonValidos_DebeEnviarCorreoDeActivacion()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        ConfigurarMocksParaFlujoExitoso();

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        _urlActivacionMock.Received(1).Construir(TokenGenerado);
        await _servicioCorreoMock.Received(1).EnviarActivacionCuentaAsync(
            destinatario: EmailValido,
            nombre:       NombreUsuario,
            enlaceActivacion: EnlaceActivacion,
            cancelacion:  Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoEmailTieneEspaciosYMayusculas_DebeNormalizarloAntesDeBuscar()
    {
        // Arrange — email con espacios y mayúsculas
        var emailSinNormalizar = "  NUEVO@GrupoXpert.COM  ";
        var emailNormalizado   = "nuevo@grupoxpert.com";
        var comando = new CrearUsuarioCommand(emailSinNormalizar, ClaveTextoPlano, NombreUsuario);

        _repositorioMock.ExisteEmailAsync(emailNormalizado, Arg.Any<CancellationToken>())
            .Returns(false);
        _servicioHashMock.GenerarHash(ClaveTextoPlano).Returns(HashGenerado);
        _generadorTokenMock.GenerarToken().Returns(TokenGenerado);
        _urlActivacionMock.Construir(TokenGenerado).Returns(EnlaceActivacion);

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert — la consulta de unicidad debe realizarse con el email normalizado
        await _repositorioMock.Received(1)
            .ExisteEmailAsync(emailNormalizado, Arg.Any<CancellationToken>());
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE UNICIDAD
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task RegistrarUsuario_CuandoEmailYaExiste_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);

        _repositorioMock.ExisteEmailAsync(EmailValido, Arg.Any<CancellationToken>())
            .Returns(true); // Email duplicado

        // Act
        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage($"El correo electrónico '{EmailValido}' ya está registrado.");
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoEmailYaExiste_NoDebeEnviarCorreoNiPersistir()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);

        _repositorioMock.ExisteEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        try { await _manejador.Handle(comando, CancellationToken.None); } catch { /* esperado */ }

        // Assert — no debe llamar a AgregarAsync ni al correo
        await _repositorioMock.DidNotReceive()
            .AgregarAsync(Arg.Any<Usuario>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajoMock.DidNotReceive()
            .GuardarCambiosAsync(Arg.Any<CancellationToken>());
        await _servicioCorreoMock.DidNotReceive()
            .EnviarActivacionCuentaAsync(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // COMANDO CON IMAGEN OPCIONAL
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task RegistrarUsuario_CuandoImagenEsProvista_DebePersistirlaJuntoAlUsuario()
    {
        // Arrange
        const string urlImagen = "https://cdn.grupoxpert.com/perfiles/avatar.png";
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario, urlImagen);
        ConfigurarMocksParaFlujoExitoso();

        Usuario? usuarioPersistido = null;
        await _repositorioMock
            .AgregarAsync(Arg.Do<Usuario>(u => usuarioPersistido = u), Arg.Any<CancellationToken>());

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        usuarioPersistido.Should().NotBeNull();
        usuarioPersistido!.Imagen.Should().Be(urlImagen);
    }

    [Fact]
    public async Task RegistrarUsuario_CuandoImagenEsNula_DebeCrearseCorrectamenteSinImagen()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario, Imagen: null);
        ConfigurarMocksParaFlujoExitoso();

        Usuario? usuarioPersistido = null;
        await _repositorioMock
            .AgregarAsync(Arg.Do<Usuario>(u => usuarioPersistido = u), Arg.Any<CancellationToken>());

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        usuarioPersistido!.Imagen.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PROPAGACIÓN DE ERRORES INESPERADOS
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task RegistrarUsuario_CuandoRepositorioFallaAlAgregar_DebePropararLaExcepcion()
    {
        // Arrange
        var comando = new CrearUsuarioCommand(EmailValido, ClaveTextoPlano, NombreUsuario);
        _repositorioMock.ExisteEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _servicioHashMock.GenerarHash(Arg.Any<string>()).Returns(HashGenerado);
        _generadorTokenMock.GenerarToken().Returns(TokenGenerado);
        _urlActivacionMock.Construir(Arg.Any<string>()).Returns(EnlaceActivacion);

        _repositorioMock
            .AgregarAsync(Arg.Any<Usuario>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => Task.FromException(new InvalidOperationException("Error de base de datos simulado")));

        // Act
        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Error de base de datos simulado");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void ConfigurarMocksParaFlujoExitoso()
    {
        _repositorioMock
            .ExisteEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _servicioHashMock.GenerarHash(ClaveTextoPlano).Returns(HashGenerado);
        _generadorTokenMock.GenerarToken().Returns(TokenGenerado);
        _urlActivacionMock.Construir(TokenGenerado).Returns(EnlaceActivacion);
    }
}
