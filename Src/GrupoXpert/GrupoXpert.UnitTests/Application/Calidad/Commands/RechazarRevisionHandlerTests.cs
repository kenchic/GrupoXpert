using FluentAssertions;
using GrupoXpert.Application.Calidad.Commands;
using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Calidad.Commands;

public class RechazarRevisionHandlerTests
{
    private readonly IRevisionCalidadRepository _revisionRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly RechazarRevisionHandler _manejador;

    public RechazarRevisionHandlerTests()
    {
        _revisionRepositorio = Substitute.For<IRevisionCalidadRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new RechazarRevisionHandler(
            _revisionRepositorio, _usuarioRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoRevisionPendiente_DebeRechazarConObservaciones()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var observaciones = "El formato APA no es correcto en las referencias.";
        var comando = new RechazarRevisionCommand(revision.Id, observaciones);

        _revisionRepositorio.ObtenerPorIdAsync(revision.Id, Arg.Any<CancellationToken>()).Returns(revision);
        _usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.VistoBueno.Should().BeFalse();
        resultado.Observaciones.Should().Be(observaciones);
        resultado.Estado.Should().Be((int)EstadoRevisionCalidad.Rechazado);

        await _revisionRepositorio.Received(1).ActualizarAsync(revision, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisionNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var revisionId = Guid.NewGuid();
        var comando = new RechazarRevisionCommand(revisionId, "Observaciones de rechazo.");

        _revisionRepositorio.ObtenerPorIdAsync(revisionId, Arg.Any<CancellationToken>()).Returns((RevisionCalidad?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*revisión de calidad no existe*");
        await _revisionRepositorio.DidNotReceive().ActualizarAsync(
            Arg.Any<RevisionCalidad>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisorExiste_DebeRetornarDtoConNombreRevisor()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var observaciones = "Documento incompleto.";
        var comando = new RechazarRevisionCommand(revision.Id, observaciones);
        var usuarioRevisor = Usuario.Crear(
            "revisor@test.com", "hash123", "Ana Revisora",
            "token-activacion-revisor", tipo: TipoUsuario.Revisor);

        _revisionRepositorio.ObtenerPorIdAsync(revision.Id, Arg.Any<CancellationToken>()).Returns(revision);
        _usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, Arg.Any<CancellationToken>()).Returns(usuarioRevisor);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.NombreRevisor.Should().Be("Ana Revisora");
        resultado.Observaciones.Should().Be(observaciones);
    }
}
