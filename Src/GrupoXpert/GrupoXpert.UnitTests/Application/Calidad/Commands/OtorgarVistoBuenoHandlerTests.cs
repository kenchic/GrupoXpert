using FluentAssertions;
using GrupoXpert.Application.Calidad.Commands;
using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Calidad.Commands;

public class OtorgarVistoBuenoHandlerTests
{
    private readonly IRevisionCalidadRepository _revisionRepositorio;
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly OtorgarVistoBuenoHandler _manejador;

    public OtorgarVistoBuenoHandlerTests()
    {
        _revisionRepositorio = Substitute.For<IRevisionCalidadRepository>();
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new OtorgarVistoBuenoHandler(
            _revisionRepositorio, _avanceRepositorio, _usuarioRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoRevisionPendiente_DebeOtorgarVistoBuenoYLiberarAvance()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Entrega final", 3, TipoAvance.Final);
        var revision = RevisionCalidad.Crear(avance.Id, Guid.NewGuid());
        var comando = new OtorgarVistoBuenoCommand(revision.Id);

        _revisionRepositorio.ObtenerPorIdAsync(revision.Id, Arg.Any<CancellationToken>()).Returns(revision);
        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);
        _usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.VistoBueno.Should().BeTrue();
        resultado.Estado.Should().Be((int)EstadoRevisionCalidad.Aprobado);
        avance.Estado.Should().Be(EstadoAvance.Liberado);

        await _revisionRepositorio.Received(1).ActualizarAsync(revision, Arg.Any<CancellationToken>());
        await _avanceRepositorio.Received(1).ActualizarAsync(avance, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisionNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var revisionId = Guid.NewGuid();
        var comando = new OtorgarVistoBuenoCommand(revisionId);

        _revisionRepositorio.ObtenerPorIdAsync(revisionId, Arg.Any<CancellationToken>()).Returns((RevisionCalidad?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*revisión de calidad no existe*");
        await _avanceRepositorio.DidNotReceive().ActualizarAsync(
            Arg.Any<Avance>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceAsociadoNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var comando = new OtorgarVistoBuenoCommand(revision.Id);

        _revisionRepositorio.ObtenerPorIdAsync(revision.Id, Arg.Any<CancellationToken>()).Returns(revision);
        _avanceRepositorio.ObtenerPorIdAsync(revision.AvanceId, Arg.Any<CancellationToken>()).Returns((Avance?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*avance asociado*");
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisorExiste_DebeRetornarDtoConNombreRevisor()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Entrega final con revisor", 3, TipoAvance.Final);
        var revision = RevisionCalidad.Crear(avance.Id, Guid.NewGuid());
        var comando = new OtorgarVistoBuenoCommand(revision.Id);
        var usuarioRevisor = Usuario.Crear(
            "revisor@test.com", "hash123", "Maria Revisora",
            "token-activacion-revisor", tipo: TipoUsuario.Revisor);

        _revisionRepositorio.ObtenerPorIdAsync(revision.Id, Arg.Any<CancellationToken>()).Returns(revision);
        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);
        _usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, Arg.Any<CancellationToken>()).Returns(usuarioRevisor);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.NombreRevisor.Should().Be("Maria Revisora");
    }
}
