using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Perfil.Commands;
using GrupoXpert.Domain.Perfil;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application;

public class ActualizarPerfilHandlerTests
{
    private readonly IPerfilClienteRepository _repositorioPerfil;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly ActualizarPerfilHandler _manejador;

    public ActualizarPerfilHandlerTests()
    {
        _repositorioPerfil = Substitute.For<IPerfilClienteRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new ActualizarPerfilHandler(_repositorioPerfil, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoPerfilNoExiste_DebeCrearNuevoYGuardar()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var comando = new ActualizarPerfilCommand(
            usuarioId,
            (int)NivelAcademico.Maestria,
            (int)UrgenciaEntrega.Urgente,
            "+573111111111",
            new List<string> { "Derecho", "Tecnología" });

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns((PerfilCliente)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        await _repositorioPerfil.Received(1).AgregarAsync(Arg.Any<PerfilCliente>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoPerfilExiste_DebeActualizarYGuardar()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var perfilExistente = PerfilCliente.Crear(usuarioId);
        var comando = new ActualizarPerfilCommand(
            usuarioId,
            (int)NivelAcademico.Doctorado,
            (int)UrgenciaEntrega.Media,
            null,
            new List<string> { "Investigación" });

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns(perfilExistente);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().BeTrue();
        perfilExistente.NivelAcademico.Should().Be(NivelAcademico.Doctorado);
        perfilExistente.AreasInteres.Should().Contain("Investigación");
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
