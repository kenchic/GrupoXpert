using FluentAssertions;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Calidad.Events;
using GrupoXpert.Domain.Exceptions;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Calidad;

public class RevisionCalidadTests
{
    [Fact]
    public void Crear_ConDatosValidos_DebeInicializarCorrectamenteYRegistrarEvento()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var revisorId = Guid.NewGuid();

        // Act
        var revision = RevisionCalidad.Crear(avanceId, revisorId);

        // Assert
        revision.AvanceId.Should().Be(avanceId);
        revision.RevisorId.Should().Be(revisorId);
        revision.VistoBueno.Should().BeFalse();
        revision.Observaciones.Should().BeNull();
        revision.Estado.Should().Be(EstadoRevisionCalidad.Pendiente);
        revision.FechaRevision.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        revision.EventosDominio.Should().ContainItemsAssignableTo<RevisionCalidadCreadaEvent>();
    }

    [Fact]
    public void Crear_ConAvanceIdVacio_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accion = () => RevisionCalidad.Crear(Guid.Empty, Guid.NewGuid());

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*avance*");
    }

    [Fact]
    public void Crear_ConRevisorIdVacio_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accion = () => RevisionCalidad.Crear(Guid.NewGuid(), Guid.Empty);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*revisor*");
    }

    [Fact]
    public void OtorgarVistoBueno_CuandoPendiente_DebeAprobarYRegistrarEvento()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());

        // Act
        revision.OtorgarVistoBueno();

        // Assert
        revision.VistoBueno.Should().BeTrue();
        revision.Estado.Should().Be(EstadoRevisionCalidad.Aprobado);
        revision.EventosDominio.Should().ContainItemsAssignableTo<VistoBuenoOtorgadoEvent>();
    }

    [Fact]
    public void OtorgarVistoBueno_CuandoYaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        revision.OtorgarVistoBueno();

        // Act
        var accion = () => revision.OtorgarVistoBueno();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes*");
    }

    [Fact]
    public void OtorgarVistoBueno_CuandoRechazado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        revision.Rechazar("El documento no cumple con los estándares de calidad.");

        // Act
        var accion = () => revision.OtorgarVistoBueno();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes*");
    }

    [Fact]
    public void Rechazar_ConObservacionesValidas_DebeRechazarYRegistrarEvento()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var observaciones = "Formato de citación incorrecto y faltan referencias bibliográficas.";

        // Act
        revision.Rechazar(observaciones);

        // Assert
        revision.VistoBueno.Should().BeFalse();
        revision.Observaciones.Should().Be(observaciones);
        revision.Estado.Should().Be(EstadoRevisionCalidad.Rechazado);
        revision.EventosDominio.Should().ContainItemsAssignableTo<RevisionRechazadaEvent>();
    }

    [Fact]
    public void Rechazar_ConObservacionesConEspaciosEnBlanco_DebeLimpiarYRechazar()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var observaciones = "  Documento incompleto.  ";

        // Act
        revision.Rechazar(observaciones);

        // Assert
        revision.Observaciones.Should().Be("Documento incompleto.");
        revision.Estado.Should().Be(EstadoRevisionCalidad.Rechazado);
    }

    [Fact]
    public void Rechazar_SinObservaciones_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var accion = () => revision.Rechazar("");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*observaciones*");
    }

    [Fact]
    public void Rechazar_ConSoloEspacios_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var accion = () => revision.Rechazar("   ");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*observaciones*");
    }

    [Fact]
    public void Rechazar_CuandoYaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        revision.OtorgarVistoBueno();

        // Act
        var accion = () => revision.Rechazar("Intento de rechazo tardío.");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes*");
    }

    [Fact]
    public void Rechazar_CuandoYaRechazado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        revision.Rechazar("Primer rechazo.");

        // Act
        var accion = () => revision.Rechazar("Segundo rechazo.");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes*");
    }

    [Fact]
    public void EventosDominio_DespuesDeCrearYAprobar_DebeContenerAmbosEventos()
    {
        // Arrange
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());

        // Act
        revision.OtorgarVistoBueno();

        // Assert
        revision.EventosDominio.Should().HaveCount(2);
        revision.EventosDominio.Should().ContainItemsAssignableTo<RevisionCalidadCreadaEvent>();
        revision.EventosDominio.Should().ContainItemsAssignableTo<VistoBuenoOtorgadoEvent>();
    }
}
