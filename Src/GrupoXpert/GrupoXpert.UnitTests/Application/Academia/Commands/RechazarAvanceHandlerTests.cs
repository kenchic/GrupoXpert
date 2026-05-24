using FluentAssertions;
using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Commands;

public class RechazarAvanceHandlerTests
{
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly RechazarAvanceHandler _manejador;

    public RechazarAvanceHandlerTests()
    {
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new RechazarAvanceHandler(_avanceRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoAvanceExiste_DebeRechazarYGuardarCambios()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance para rechazar", 1, TipoAvance.Parcial);
        var comando = new RechazarAvanceCommand(avance.Id);

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>())
            .Returns(avance);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        avance.Estado.Should().Be(EstadoAvance.Rechazado);

        await _avanceRepositorio.Received(1).ActualizarAsync(avance, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var comando = new RechazarAvanceCommand(avanceId);

        _avanceRepositorio.ObtenerPorIdAsync(avanceId, Arg.Any<CancellationToken>())
            .Returns((Avance?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*avance no existe*");

        await _avanceRepositorio.DidNotReceive().ActualizarAsync(Arg.Any<Avance>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
