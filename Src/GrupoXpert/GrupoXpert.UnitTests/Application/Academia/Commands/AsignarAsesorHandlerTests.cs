using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Commands;

public class AsignarAsesorHandlerTests
{
    private readonly ISolicitudAcademicaRepository _repositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly AsignarAsesorHandler _manejador;

    public AsignarAsesorHandlerTests()
    {
        _repositorio = Substitute.For<ISolicitudAcademicaRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new AsignarAsesorHandler(_repositorio, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteYEstaPendiente_DebeAsignarAsesorYGuardarCambios()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId,
            NivelAcademico.Pregrado,
            TipoTrabajo.Tesis,
            "Derecho",
            DateTime.UtcNow.AddDays(5),
            10,
            NormaCitacion.APA,
            IdiomaRequerido.Espanol,
            "PDF",
            "Instrucciones",
            false,
            false
        );
        
        var solicitudId = solicitud.Id;
        var asesorId = Guid.NewGuid();
        var comando = new AsignarAsesorCommand(solicitudId, asesorId);

        _repositorio.ObtenerPorIdAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        solicitud.AsesorId.Should().Be(asesorId);
        solicitud.Estado.Should().Be(EstadoSolicitud.Asignada);

        await _repositorio.Received(1).ActualizarAsync(solicitud, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var solicitudId = Guid.NewGuid();
        var asesorId = Guid.NewGuid();
        var comando = new AsignarAsesorCommand(solicitudId, asesorId);

        _repositorio.ObtenerPorIdAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns((SolicitudAcademica)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("La solicitud académica no existe.");

        await _repositorio.DidNotReceive().ActualizarAsync(Arg.Any<SolicitudAcademica>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
