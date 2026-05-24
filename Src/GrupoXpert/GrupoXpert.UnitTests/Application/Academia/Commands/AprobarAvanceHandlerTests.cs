using FluentAssertions;
using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Commands;

public class AprobarAvanceHandlerTests
{
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly AprobarAvanceHandler _manejador;

    public AprobarAvanceHandlerTests()
    {
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new AprobarAvanceHandler(_avanceRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoAvanceExiste_DebeAprobarYGuardarCambios()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance para aprobar", 1, TipoAvance.Parcial);
        var comando = new AprobarAvanceCommand(avance.Id);

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>())
            .Returns(avance);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        avance.Estado.Should().Be(EstadoAvance.Aprobado);

        await _avanceRepositorio.Received(1).ActualizarAsync(avance, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var comando = new AprobarAvanceCommand(avanceId);

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
