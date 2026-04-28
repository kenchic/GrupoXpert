using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Identidad.Commands;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Identidad.Commands;

public sealed class IniciarSesionHandlerTests
{
    private readonly IUsuarioRepository _usuarioRepositoryMock;
    private readonly IHashClaveService _hashClaveServiceMock;
    private readonly ITokenService _tokenServiceMock;
    private readonly IUnidadDeTrabajo _unidadDeTrabajoMock;
    private readonly IniciarSesionHandler _manejador;

    public IniciarSesionHandlerTests()
    {
        _usuarioRepositoryMock = Substitute.For<IUsuarioRepository>();
        _hashClaveServiceMock = Substitute.For<IHashClaveService>();
        _tokenServiceMock = Substitute.For<ITokenService>();
        _unidadDeTrabajoMock = Substitute.For<IUnidadDeTrabajo>();

        _manejador = new IniciarSesionHandler(
            _usuarioRepositoryMock,
            _hashClaveServiceMock,
            _tokenServiceMock,
            _unidadDeTrabajoMock);
    }

    [Fact]
    public async Task IniciarSesion_CuandoCredencialesSonCorrectas_DebeRetornarToken()
    {
        // Arrange
        var comando = new IniciarSesionComando("admin", "TuPassword123!");
        var usuario = Usuario.Crear("admin", "hash_simulado", "Administrador", null);
        var tokenEsperado = "token_jwt_valido";

        _usuarioRepositoryMock.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, Arg.Any<CancellationToken>())
            .Returns(usuario);
        
        _hashClaveServiceMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(true);
            
        _tokenServiceMock.GenerarToken(usuario)
            .Returns(tokenEsperado);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().Be(tokenEsperado);
        resultado.NombreUsuario.Should().Be(usuario.NombreUsuario);
        
        // Verificar que se llamó al repositorio y a la unidad de trabajo
        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
        
        // Verificar que se registró el inicio de sesión
        usuario.UltimoInicioSesion.Should().NotBeNull();
    }

    [Fact]
    public async Task IniciarSesion_CuandoUsuarioNoExiste_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var comando = new IniciarSesionComando("usuario_inexistente", "clave");

        _usuarioRepositoryMock.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, Arg.Any<CancellationToken>())
            .Returns((Usuario?)null);

        // Act
        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task IniciarSesion_CuandoClaveEsIncorrecta_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var comando = new IniciarSesionComando("admin", "clave_incorrecta");
        var usuario = Usuario.Crear("admin", "hash_real", "Administrador");

        _usuarioRepositoryMock.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, Arg.Any<CancellationToken>())
            .Returns(usuario);
            
        _hashClaveServiceMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(false);

        // Act
        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task IniciarSesion_CuandoUsuarioEstaInactivo_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var comando = new IniciarSesionComando("admin", "TuPassword123!");
        var usuario = Usuario.Crear("admin", "hash_simulado", "Administrador");
        usuario.Desactivar(); // Hacemos que la cuenta esté inactiva

        _usuarioRepositoryMock.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, Arg.Any<CancellationToken>())
            .Returns(usuario);
            
        _hashClaveServiceMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(true);

        // Act
        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("No se puede iniciar sesión con una cuenta inactiva.");
    }
}
