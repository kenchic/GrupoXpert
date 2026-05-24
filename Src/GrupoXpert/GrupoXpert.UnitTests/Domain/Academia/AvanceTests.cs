using FluentAssertions;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Academia.Events;
using GrupoXpert.Domain.Exceptions;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Academia;

public class AvanceTests
{
    [Fact]
    public void Subir_ConDatosValidos_DebeInicializarCorrectamenteYRegistrarEvento()
    {
        // Arrange
        var solicitudId = Guid.NewGuid();
        var asesorId = Guid.NewGuid();
        var descripcion = "Avance del capítulo 1 de la tesis";
        var numeroFase = 1;
        var tipo = TipoAvance.Parcial;

        // Act
        var avance = Avance.Subir(solicitudId, asesorId, descripcion, numeroFase, tipo);

        // Assert
        avance.SolicitudId.Should().Be(solicitudId);
        avance.AsesorId.Should().Be(asesorId);
        avance.Descripcion.Should().Be(descripcion);
        avance.NumeroFase.Should().Be(numeroFase);
        avance.Tipo.Should().Be(tipo);
        avance.Estado.Should().Be(EstadoAvance.PendienteRevision);
        avance.FechaSubida.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        avance.Comentarios.Should().BeEmpty();
        avance.ArchivosAdjuntos.Should().BeEmpty();
        avance.EventosDominio.Should().ContainItemsAssignableTo<AvanceSubidoDomainEvent>();
    }

    [Fact]
    public void Subir_ConTipoFinal_DebeInicializarConTipoCorrecto()
    {
        // Arrange
        var solicitudId = Guid.NewGuid();
        var asesorId = Guid.NewGuid();
        var descripcion = "Entrega final de la tesis completa";
        var numeroFase = 3;
        var tipo = TipoAvance.Final;

        // Act
        var avance = Avance.Subir(solicitudId, asesorId, descripcion, numeroFase, tipo);

        // Assert
        avance.Tipo.Should().Be(TipoAvance.Final);
        avance.NumeroFase.Should().Be(3);
    }

    [Fact]
    public void Subir_ConSolicitudIdVacio_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accion = () => Avance.Subir(
            Guid.Empty, Guid.NewGuid(), "Descripción válida", 1, TipoAvance.Parcial);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*solicitud*");
    }

    [Fact]
    public void Subir_ConAsesorIdVacio_DebeLanzarArgumentException()
    {
        // Arrange & Act
        var accion = () => Avance.Subir(
            Guid.NewGuid(), Guid.Empty, "Descripción válida", 1, TipoAvance.Parcial);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*asesor*");
    }

    [Fact]
    public void Subir_ConDescripcionVacia_DebeLanzarExcepcionDominio()
    {
        // Arrange & Act
        var accion = () => Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "  ", 1, TipoAvance.Parcial);

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*descripción*");
    }

    [Fact]
    public void Subir_ConNumeroFaseMenorAUno_DebeLanzarExcepcionDominio()
    {
        // Arrange & Act
        var accion = () => Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Descripción", 0, TipoAvance.Parcial);

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*fase*");
    }

    [Fact]
    public void AgregarComentario_ConDatosValidos_DebeAgregarComentarioYRegistrarEvento()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);
        var autorId = Guid.NewGuid();
        var contenido = "Excelente avance, sigue así.";

        // Act
        var comentario = avance.AgregarComentario(autorId, contenido);

        // Assert
        comentario.AutorId.Should().Be(autorId);
        comentario.Contenido.Should().Be(contenido);
        comentario.AvanceId.Should().Be(avance.Id);
        avance.Comentarios.Should().HaveCount(1);
        avance.EventosDominio.Should().ContainItemsAssignableTo<ComentarioAgregadoDomainEvent>();
    }

    [Fact]
    public void AgregarComentario_VariosComentarios_DebeAcumularCorrectamente()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);
        var autor1 = Guid.NewGuid();
        var autor2 = Guid.NewGuid();

        // Act
        avance.AgregarComentario(autor1, "Primer comentario");
        avance.AgregarComentario(autor2, "Segundo comentario");

        // Assert
        avance.Comentarios.Should().HaveCount(2);
    }

    [Fact]
    public void AgregarComentario_ConAutorIdVacio_DebeLanzarArgumentException()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);

        // Act
        var accion = () => avance.AgregarComentario(Guid.Empty, "Contenido válido");

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*autor*");
    }

    [Fact]
    public void AgregarComentario_ConContenidoVacio_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);

        // Act
        var accion = () => avance.AgregarComentario(Guid.NewGuid(), "");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*comentario*");
    }

    [Fact]
    public void AgregarArchivo_ConDatosValidos_DebeAgregarCorrectamente()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance con archivos", 1, TipoAvance.Parcial);
        var nombreArchivo = "capitulo1.pdf";
        var url = "https://storage.grupoxpert.com/archivos/cap1.pdf";
        var tamanioBytes = 1024000L;
        var tipoContenido = "application/pdf";

        // Act
        var archivo = avance.AgregarArchivo(nombreArchivo, url, tamanioBytes, tipoContenido);

        // Assert
        archivo.NombreArchivo.Should().Be(nombreArchivo);
        archivo.Url.Should().Be(url);
        archivo.TamanioBytes.Should().Be(tamanioBytes);
        archivo.TipoContenido.Should().Be(tipoContenido);
        archivo.AvanceId.Should().Be(avance.Id);
        avance.ArchivosAdjuntos.Should().HaveCount(1);
    }

    [Fact]
    public void AgregarArchivo_VariosArchivos_DebeAcumularCorrectamente()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance con múltiples archivos", 1, TipoAvance.Parcial);

        // Act
        avance.AgregarArchivo("archivo1.pdf", "https://url.com/1", 1000, "application/pdf");
        avance.AgregarArchivo("archivo2.docx", "https://url.com/2", 2000, "application/docx");

        // Assert
        avance.ArchivosAdjuntos.Should().HaveCount(2);
    }

    [Fact]
    public void AgregarArchivo_ConNombreVacio_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);

        // Act
        var accion = () => avance.AgregarArchivo("", "https://url.com", 1000, "application/pdf");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*nombre del archivo*");
    }

    [Fact]
    public void AgregarArchivo_ConUrlVacia_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);

        // Act
        var accion = () => avance.AgregarArchivo("archivo.pdf", "  ", 1000, "application/pdf");

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*URL*");
    }

    [Fact]
    public void AgregarArchivo_ConTamanioCeroOMenor_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance inicial", 1, TipoAvance.Parcial);

        // Act
        var accionCero = () => avance.AgregarArchivo("a.pdf", "https://url.com", 0, "application/pdf");
        var accionNegativo = () => avance.AgregarArchivo("b.pdf", "https://url.com", -5, "application/pdf");

        // Assert
        accionCero.Should().Throw<ExcepcionDominio>().WithMessage("*tamaño*");
        accionNegativo.Should().Throw<ExcepcionDominio>().WithMessage("*tamaño*");
    }

    [Fact]
    public void Aprobar_CuandoEstaPendienteRevision_DebeCambiarEstadoAAprobado()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance para aprobar", 1, TipoAvance.Parcial);

        // Act
        avance.Aprobar();

        // Assert
        avance.Estado.Should().Be(EstadoAvance.Aprobado);
    }

    [Fact]
    public void Aprobar_CuandoYaEstaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance ya aprobado", 1, TipoAvance.Parcial);
        avance.Aprobar();

        // Act
        var accion = () => avance.Aprobar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes de revisión*");
    }

    [Fact]
    public void Aprobar_CuandoEstaRechazado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance ya rechazado", 1, TipoAvance.Parcial);
        avance.Rechazar();

        // Act
        var accion = () => avance.Aprobar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes de revisión*");
    }

    [Fact]
    public void Rechazar_CuandoEstaPendienteRevision_DebeCambiarEstadoARechazado()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance para rechazar", 1, TipoAvance.Parcial);

        // Act
        avance.Rechazar();

        // Assert
        avance.Estado.Should().Be(EstadoAvance.Rechazado);
    }

    [Fact]
    public void Rechazar_CuandoYaEstaRechazado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance ya rechazado", 1, TipoAvance.Parcial);
        avance.Rechazar();

        // Act
        var accion = () => avance.Rechazar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes de revisión*");
    }

    [Fact]
    public void Rechazar_CuandoEstaAprobado_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var avance = Avance.Subir(
            Guid.NewGuid(), Guid.NewGuid(), "Avance ya aprobado", 1, TipoAvance.Parcial);
        avance.Aprobar();

        // Act
        var accion = () => avance.Rechazar();

        // Assert
        accion.Should().Throw<ExcepcionDominio>().WithMessage("*pendientes de revisión*");
    }
}
