using System;
using FluentAssertions;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Academia.Events;
using GrupoXpert.Domain.Exceptions;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Academia;

public class SolicitudAcademicaTests
{
    [Fact]
    public void Crear_ConDatosValidos_DebeInicializarCorrectamenteYRegistrarEvento()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var nivel = NivelAcademico.Pregrado;
        var tipo = TipoTrabajo.Tesis;
        var area = "Derecho Penal";
        var fecha = DateTime.UtcNow.AddDays(15).Date;
        var paginas = 50;
        var norma = NormaCitacion.APA;
        var idioma = IdiomaRequerido.Espanol;
        var formato = "Word Arial 12";
        var material = "Material de lectura base adjunto";
        var esUrgente = true;
        var fases = false;

        // Act
        var solicitud = SolicitudAcademica.Crear(
            clienteId, nivel, tipo, area, fecha, paginas, norma, idioma, formato, material, esUrgente, fases);

        // Assert
        solicitud.ClienteId.Should().Be(clienteId);
        solicitud.NivelAcademico.Should().Be(nivel);
        solicitud.TipoTrabajo.Should().Be(tipo);
        solicitud.AreaTematica.Should().Be(area);
        solicitud.FechaEntrega.Should().Be(fecha);
        solicitud.NumeroPaginasOPalabras.Should().Be(paginas);
        solicitud.NormaCitacion.Should().Be(norma);
        solicitud.Idioma.Should().Be(idioma);
        solicitud.FormatoRequerido.Should().Be(formato);
        solicitud.MaterialBase.Should().Be(material);
        solicitud.EsUrgente.Should().Be(esUrgente);
        solicitud.EntregaPorFases.Should().Be(fases);
        solicitud.Estado.Should().Be(EstadoSolicitud.Pendiente);
        solicitud.AsesorId.Should().BeNull();
        solicitud.EventosDominio.Should().ContainItemsAssignableTo<SolicitudAcademicaCreadaDomainEvent>();
    }

    [Fact]
    public void Crear_ConFechaEnElPasado_DebeLanzarArgumentException()
    {
        // Arrange
        var fechaPasada = DateTime.UtcNow.AddDays(-1).Date;

        // Act
        var accion = () => SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", fechaPasada, 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*fecha de entrega*");
    }

    [Fact]
    public void Crear_ConNumeroPaginasCeroOMenor_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accionCero = () => SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 0, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
            
        var accionNegativo = () => SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), -5, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Assert
        accionCero.Should().Throw<ArgumentException>().WithMessage("*páginas o palabras*");
        accionNegativo.Should().Throw<ArgumentException>().WithMessage("*páginas o palabras*");
    }

    [Fact]
    public void Crear_ConAreaTematicaVacia_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accionVacia = () => SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "  ", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Assert
        accionVacia.Should().Throw<ArgumentException>().WithMessage("*área temática*");
    }

    [Fact]
    public void Crear_ConClienteIdVacio_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accionVacio = () => SolicitudAcademica.Crear(
            Guid.Empty, NivelAcademico.Pregrado, TipoTrabajo.Tesis, "Derecho", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Assert
        accionVacio.Should().Throw<ArgumentException>().WithMessage("*cliente es obligatorio*");
    }

    [Fact]
    public void AsignarAsesor_ConAsesorIdValido_DebeAsignarYCambiarEstadoAEnProceso()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        var asesorId = Guid.NewGuid();

        // Act
        solicitud.AsignarAsesor(asesorId);

        // Assert
        solicitud.AsesorId.Should().Be(asesorId);
        solicitud.Estado.Should().Be(EstadoSolicitud.EnProceso);
    }

    [Fact]
    public void AsignarAsesor_ConAsesorIdVacio_DebeLanzarArgumentException()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Act
        var accion = () => solicitud.AsignarAsesor(Guid.Empty);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*asesor*");
    }

    [Fact]
    public void AsignarAsesor_CuandoNoEstaPendiente_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        solicitud.AsignarAsesor(Guid.NewGuid()); // Pasa a EnProceso

        // Act
        var accion = () => solicitud.AsignarAsesor(Guid.NewGuid());

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendiente*");
    }

    [Fact]
    public void Completar_CuandoEstaEnProceso_DebeCambiarEstadoACompletada()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        solicitud.AsignarAsesor(Guid.NewGuid()); // Pasa a EnProceso

        // Act
        solicitud.Completar();

        // Assert
        solicitud.Estado.Should().Be(EstadoSolicitud.Completada);
    }

    [Fact]
    public void Completar_CuandoNoEstaEnProceso_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);

        // Act
        var accion = () => solicitud.Completar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*proceso*");
    }

    [Fact]
    public void Cancelar_CuandoNoEstaCompletada_DebeCambiarEstadoACancelada()
    {
        // Arrange & Act & Assert
        var solicitud1 = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        solicitud1.Cancelar();
        solicitud1.Estado.Should().Be(EstadoSolicitud.Cancelada);

        var solicitud2 = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        solicitud2.AsignarAsesor(Guid.NewGuid());
        solicitud2.Cancelar();
        solicitud2.Estado.Should().Be(EstadoSolicitud.Cancelada);
    }

    [Fact]
    public void Cancelar_CuandoYaEstaCompletada_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "IA", DateTime.UtcNow.AddDays(5), 10, NormaCitacion.APA, IdiomaRequerido.Espanol, "PDF", "Instrucciones", false, false);
        solicitud.AsignarAsesor(Guid.NewGuid());
        solicitud.Completar();

        // Act
        var accion = () => solicitud.Cancelar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*completada*");
    }
}
