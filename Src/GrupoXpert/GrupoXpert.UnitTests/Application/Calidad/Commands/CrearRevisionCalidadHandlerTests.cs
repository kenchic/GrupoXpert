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

public class CrearRevisionCalidadHandlerTests
{
    private readonly IRevisionCalidadRepository _revisionRepositorio;
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly CrearRevisionCalidadHandler _manejador;

    public CrearRevisionCalidadHandlerTests()
    {
        _revisionRepositorio = Substitute.For<IRevisionCalidadRepository>();
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new CrearRevisionCalidadHandler(
            _revisionRepositorio, _avanceRepositorio, _usuarioRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoAvanceFinalExiste_DebeCrearRevisionYGuardarCambios()
    {
        // Arrange
        var revisorId = Guid.NewGuid();
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Entrega final para revisión", 3, TipoAvance.Final);
        var comando = new CrearRevisionCalidadCommand(avance.Id, revisorId);

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);
        _revisionRepositorio.ObtenerPorAvanceAsync(avance.Id, Arg.Any<CancellationToken>()).Returns((RevisionCalidad?)null);
        _usuarioRepositorio.ObtenerPorIdAsync(revisorId, Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.AvanceId.Should().Be(avance.Id);
        resultado.RevisorId.Should().Be(revisorId);
        resultado.VistoBueno.Should().BeFalse();
        resultado.Estado.Should().Be((int)EstadoRevisionCalidad.Pendiente);

        await _revisionRepositorio.Received(1).AgregarAsync(
            Arg.Is<RevisionCalidad>(r => r.AvanceId == avance.Id && r.RevisorId == revisorId),
            Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var comando = new CrearRevisionCalidadCommand(avanceId, Guid.NewGuid());

        _avanceRepositorio.ObtenerPorIdAsync(avanceId, Arg.Any<CancellationToken>()).Returns((Avance?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*avance no existe*");
        await _revisionRepositorio.DidNotReceive().AgregarAsync(
            Arg.Any<RevisionCalidad>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceNoEsFinal_DebeLanzarExcepcion()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance parcial", 1, TipoAvance.Parcial);
        var comando = new CrearRevisionCalidadCommand(avance.Id, Guid.NewGuid());

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*tipo final*");
        await _revisionRepositorio.DidNotReceive().AgregarAsync(
            Arg.Any<RevisionCalidad>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoYaExisteRevisionParaAvance_DebeLanzarExcepcion()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Entrega final duplicada", 3, TipoAvance.Final);
        var revisionExistente = RevisionCalidad.Crear(avance.Id, Guid.NewGuid());
        var comando = new CrearRevisionCalidadCommand(avance.Id, Guid.NewGuid());

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);
        _revisionRepositorio.ObtenerPorAvanceAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(revisionExistente);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Ya existe*");
        await _revisionRepositorio.DidNotReceive().AgregarAsync(
            Arg.Any<RevisionCalidad>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisorExiste_DebeRetornarDtoConNombreRevisor()
    {
        // Arrange
        var revisorId = Guid.NewGuid();
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Entrega final con revisor", 3, TipoAvance.Final);
        var comando = new CrearRevisionCalidadCommand(avance.Id, revisorId);
        var usuarioRevisor = Usuario.Crear(
            "revisor@test.com", "hash123", "Carlos Revisor",
            "token-activacion-revisor", tipo: TipoUsuario.Revisor);

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>()).Returns(avance);
        _revisionRepositorio.ObtenerPorAvanceAsync(avance.Id, Arg.Any<CancellationToken>()).Returns((RevisionCalidad?)null);
        _usuarioRepositorio.ObtenerPorIdAsync(revisorId, Arg.Any<CancellationToken>()).Returns(usuarioRevisor);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NombreRevisor.Should().Be("Carlos Revisor");
    }
}
