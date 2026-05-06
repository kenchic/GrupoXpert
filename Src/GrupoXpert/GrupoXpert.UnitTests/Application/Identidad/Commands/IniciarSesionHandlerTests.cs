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
    private readonly IUsuarioRepository _repositorioUsuarioMock;
    private readonly IHashClaveService _servicioHashClaveMock;
    private readonly ITokenAccesoService _servicioTokenAccesoMock;
    private readonly IUnidadDeTrabajo _unidadDeTrabajoMock;
    private readonly IniciarSesionHandler _manejador;

    private const string CorreoPrueba = "admin@grupoxpert.com";
    private const string TokenActivacionPrueba = "token-activacion-test-123";

    public IniciarSesionHandlerTests()
    {
        _repositorioUsuarioMock = Substitute.For<IUsuarioRepository>();
        _servicioHashClaveMock = Substitute.For<IHashClaveService>();
        _servicioTokenAccesoMock = Substitute.For<ITokenAccesoService>();
        _unidadDeTrabajoMock = Substitute.For<IUnidadDeTrabajo>();

        _manejador = new IniciarSesionHandler(
            _repositorioUsuarioMock,
            _servicioHashClaveMock,
            _servicioTokenAccesoMock,
            _unidadDeTrabajoMock);
    }

    [Fact]
    public async Task IniciarSesion_CuandoCredencialesSonCorrectas_DebeRetornarToken()
    {
        var comando = new IniciarSesionCommand(CorreoPrueba, "TuPassword123!");
        var usuario = Usuario.Crear(CorreoPrueba, "hash_simulado", "Administrador", TokenActivacionPrueba, tipo: TipoUsuario.Estudiante);
        usuario.ActivarCuenta(TokenActivacionPrueba);
        var tokenEsperado = "token_jwt_valido";

        _repositorioUsuarioMock.ObtenerPorCorreoAsync(comando.Correo, Arg.Any<CancellationToken>())
            .Returns(usuario);

        _servicioHashClaveMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(true);

        _servicioTokenAccesoMock.GenerarToken(usuario)
            .Returns(tokenEsperado);

        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.TokenAcceso.Should().Be(tokenEsperado);
        resultado.Correo.Should().Be(usuario.Correo.Valor);

        await _unidadDeTrabajoMock.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());

        usuario.UltimoInicioSesion.Should().NotBeNull();
    }

    [Fact]
    public async Task IniciarSesion_CuandoUsuarioNoExiste_DebeLanzarExcepcionDominio()
    {
        var comando = new IniciarSesionCommand("inexistente@test.com", "clave");

        _repositorioUsuarioMock.ObtenerPorCorreoAsync(comando.Correo, Arg.Any<CancellationToken>())
            .Returns((Usuario?)null);

        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task IniciarSesion_CuandoClaveEsIncorrecta_DebeLanzarExcepcionDominio()
    {
        var comando = new IniciarSesionCommand(CorreoPrueba, "clave_incorrecta");
        var usuario = Usuario.Crear(CorreoPrueba, "hash_real", "Administrador", TokenActivacionPrueba, tipo: TipoUsuario.Asesor);
        usuario.ActivarCuenta(TokenActivacionPrueba);

        _repositorioUsuarioMock.ObtenerPorCorreoAsync(comando.Correo, Arg.Any<CancellationToken>())
            .Returns(usuario);

        _servicioHashClaveMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(false);

        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Credenciales inválidas.");
    }

    [Fact]
    public async Task IniciarSesion_CuandoUsuarioEstaInactivo_DebeLanzarExcepcionDominio()
    {
        var comando = new IniciarSesionCommand(CorreoPrueba, "TuPassword123!");
        var usuario = Usuario.Crear(CorreoPrueba, "hash_simulado", "Administrador", TokenActivacionPrueba, tipo: TipoUsuario.Estudiante);

        _repositorioUsuarioMock.ObtenerPorCorreoAsync(comando.Correo, Arg.Any<CancellationToken>())
            .Returns(usuario);

        _servicioHashClaveMock.Verificar(comando.Clave, usuario.Clave.HashClave)
            .Returns(true);

        Func<Task> accion = async () => await _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("La cuenta no está activa. Por favor verifica tu correo para activarla.");
    }
}
