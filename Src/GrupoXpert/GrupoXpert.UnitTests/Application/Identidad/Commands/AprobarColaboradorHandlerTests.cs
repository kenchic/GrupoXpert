using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Identidad.Commands;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Identidad.Commands;

/// <summary>
/// Pruebas de la capa de Aplicación para el comando AprobarColaborador.
/// Validan la orquestación del handler con mocks de repositorio y servicios.
/// </summary>
public sealed class AprobarColaboradorHandlerTests
{
    private readonly IUsuarioRepository _repositorio = Substitute.For<IUsuarioRepository>();
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
    private readonly ICorreoElectronicoService _correoService = Substitute.For<ICorreoElectronicoService>();

    private AprobarColaboradorHandler CrearHandler() =>
        new(_repositorio, _unidadDeTrabajo, _correoService);

    private static Usuario CrearAsesorPendiente()
    {
        return Usuario.Crear(
            "asesor@test.com", "hash_seguro", "Asesor Prueba",
            "token-activacion-123", tipo: TipoUsuario.Asesor);
    }

    [Fact]
    public async Task Handle_CuandoColaboradorExiste_DebeAprobarYGuardar()
    {
        // Arrange
        var asesor = CrearAsesorPendiente();
        var administradorId = Guid.NewGuid();
        var comando = new AprobarColaboradorCommand(asesor.Id, administradorId);

        _repositorio.ObtenerPorIdAsync(asesor.Id, Arg.Any<CancellationToken>())
            .Returns(asesor);

        var handler = CrearHandler();

        // Act
        await handler.Handle(comando, CancellationToken.None);

        // Assert
        asesor.EstaAprobado.Should().BeTrue();
        asesor.AprobadoPorId.Should().Be(administradorId);

        await _repositorio.Received(1).ActualizarAsync(asesor, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoColaboradorExiste_DebeEnviarCorreoDeAprobacion()
    {
        // Arrange
        var asesor = CrearAsesorPendiente();
        var comando = new AprobarColaboradorCommand(asesor.Id, Guid.NewGuid());

        _repositorio.ObtenerPorIdAsync(asesor.Id, Arg.Any<CancellationToken>())
            .Returns(asesor);

        var handler = CrearHandler();

        // Act
        await handler.Handle(comando, CancellationToken.None);

        // Assert
        await _correoService.Received(1).EnviarAprobacionCuentaAsync(
            asesor.Correo.Valor, asesor.Nombre, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoColaboradorNoExiste_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var comando = new AprobarColaboradorCommand(idInexistente, Guid.NewGuid());

        _repositorio.ObtenerPorIdAsync(idInexistente, Arg.Any<CancellationToken>())
            .Returns((Usuario?)null);

        var handler = CrearHandler();

        // Act
        var accion = () => handler.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Colaborador no encontrado.");

        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
