using FluentAssertions;
using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Commands;

public class AgregarComentarioHandlerTests
{
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly AgregarComentarioHandler _manejador;

    public AgregarComentarioHandlerTests()
    {
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new AgregarComentarioHandler(_avanceRepositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoAvanceExiste_DebeAgregarComentarioYGuardarCambios()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);
        var autorId = Guid.NewGuid();
        var contenido = "Buen trabajo, necesito ajustes en la introducción.";
        var comando = new AgregarComentarioCommand(avance.Id, autorId, contenido);

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>())
            .Returns(avance);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        avance.Comentarios.Should().HaveCount(1);
        avance.Comentarios.First().AutorId.Should().Be(autorId);
        avance.Comentarios.First().Contenido.Should().Be(contenido);

        await _avanceRepositorio.Received(1).ActualizarAsync(avance, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAvanceNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var comando = new AgregarComentarioCommand(avanceId, Guid.NewGuid(), "Comentario");

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

    [Fact]
    public async Task Handle_MultiplesComentarios_DebeAcumularCorrectamente()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance con discusión", 1, TipoAvance.Parcial);
        var autor1 = Guid.NewGuid();
        var autor2 = Guid.NewGuid();

        _avanceRepositorio.ObtenerPorIdAsync(avance.Id, Arg.Any<CancellationToken>())
            .Returns(avance);

        // Act
        await _manejador.Handle(new AgregarComentarioCommand(avance.Id, autor1, "Primero"), CancellationToken.None);
        await _manejador.Handle(new AgregarComentarioCommand(avance.Id, autor2, "Segundo"), CancellationToken.None);

        // Assert
        avance.Comentarios.Should().HaveCount(2);
        avance.Comentarios.ElementAt(0).AutorId.Should().Be(autor1);
        avance.Comentarios.ElementAt(1).AutorId.Should().Be(autor2);
    }
}
