using FluentAssertions;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Calificacion.Events;
using GrupoXpert.Domain.Exceptions;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Calificacion;

/// <summary>
/// Pruebas unitarias del Aggregate Root CalificacionColaborador.
/// Validan las reglas de negocio de la calificación:IDs obligatorios,
/// autocalificación prohibida, y observaciones opcionales.
/// </summary>
public sealed class CalificacionColaboradorTests
{
    // ── Constantes de prueba ─────────────────────────────────────────────────
    private static readonly Guid SolicitudIdValida = Guid.NewGuid();
    private static readonly Guid ClienteIdValido = Guid.NewGuid();
    private static readonly Guid ColaboradorIdValido = Guid.NewGuid();
    private const int PuntajeValido = 4;
    private const string ObservacionValida = "Excelente colaborador";

    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN EXITOSA (CALIFICAR)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Calificar_CuandoDatosSonValidos_DebeCrearCalificacionCorrectamente()
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, ObservacionValida);

        // Assert
        calificacion.Should().NotBeNull();
        calificacion.SolicitudId.Should().Be(SolicitudIdValida);
        calificacion.ClienteId.Should().Be(ClienteIdValido);
        calificacion.ColaboradorId.Should().Be(ColaboradorIdValido);
        calificacion.Puntaje.Valor.Should().Be(PuntajeValido);
        calificacion.Observacion.Should().Be(ObservacionValida);
    }

    [Fact]
    public void Calificar_CuandoObservacionEsNula_DebeCrearCalificacionSinObservacion()
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, null);

        // Assert
        calificacion.Observacion.Should().BeNull();
    }

    [Fact]
    public void Calificar_CuandoObservacionEsSoloEspacios_DebeGuardarComoNula()
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, "   ");

        // Assert
        calificacion.Observacion.Should().BeNull();
    }

    [Fact]
    public void Calificar_CuandoObservacionTieneEspaciosAlrededor_DebeRecortar()
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, "  Buena atención  ");

        // Assert
        calificacion.Observacion.Should().Be("Buena atención");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Calificar_CuandoPuntajeEstaEnRango_DebeCrearCalificacion(int puntaje)
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            puntaje, null);

        // Assert
        calificacion.Puntaje.Valor.Should().Be(puntaje);
    }

    [Fact]
    public void Calificar_CuandoDatosSonValidos_DebeGenerarFechaCalificacionReciente()
    {
        // Arrange
        var antesDeCrear = DateTime.UtcNow;

        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, null);

        // Assert
        var despuesDeCrear = DateTime.UtcNow;
        calificacion.FechaCalificacion.Should().BeOnOrAfter(antesDeCrear);
        calificacion.FechaCalificacion.Should().BeOnOrBefore(despuesDeCrear);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // EVENTO DE DOMINIO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Calificar_CuandoDatosSonValidos_DebeGenerarEventoColaboradorCalificado()
    {
        // Act
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, null);

        // Assert
        calificacion.EventosDominio.Should().HaveCount(1);
        var evento = calificacion.EventosDominio[0].Should().BeOfType<ColaboradorCalificadoEvent>().Subject;
        evento.SolicitudId.Should().Be(SolicitudIdValida);
        evento.ClienteId.Should().Be(ClienteIdValido);
        evento.ColaboradorId.Should().Be(ColaboradorIdValido);
        evento.Puntaje.Should().Be(PuntajeValido);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE_IDS_OBLIGATORIOS
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Calificar_CuandoSolicitudIdEsVacio_DebeLanzarArgumentException()
    {
        // Act
        var accion = () => CalificacionColaborador.Calificar(
            Guid.Empty, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, null);

        // Assert
        accion.Should().Throw<ArgumentException>()
            .WithMessage("*solicitud*");
    }

    [Fact]
    public void Calificar_CuandoClienteIdEsVacio_DebeLanzarArgumentException()
    {
        // Act
        var accion = () => CalificacionColaborador.Calificar(
            SolicitudIdValida, Guid.Empty, ColaboradorIdValido,
            PuntajeValido, null);

        // Assert
        accion.Should().Throw<ArgumentException>()
            .WithMessage("*cliente*");
    }

    [Fact]
    public void Calificar_CuandoColaboradorIdEsVacio_DebeLanzarArgumentException()
    {
        // Act
        var accion = () => CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, Guid.Empty,
            PuntajeValido, null);

        // Assert
        accion.Should().Throw<ArgumentException>()
            .WithMessage("*colaborador*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // AUTOCALIFICACIÓN_PROHIBIDA
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Calificar_CuandoClienteYColaboradorSonIguales_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var mismoId = Guid.NewGuid();

        // Act
        var accion = () => CalificacionColaborador.Calificar(
            SolicitudIdValida, mismoId, mismoId,
            PuntajeValido, null);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*no puede calificarse a sí mismo*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PUNTAJE FUERA DE RANGO (delegado a Value Object Puntaje)
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    [InlineData(100)]
    public void Calificar_CuandoPuntajeFueraDeRango_DebeLanzarExcepcionDominio(int puntajeInvalido)
    {
        // Act
        var accion = () => CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            puntajeInvalido, null);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*puntaje*1*5*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ACTUALIZAR OBSERVACIÓN
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void ActualizarObservacion_CuandoTextoEsValido_DebeActualizarObservacion()
    {
        // Arrange
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, "Observación original");

        // Act
        calificacion.ActualizarObservacion("Nueva observación");

        // Assert
        calificacion.Observacion.Should().Be("Nueva observación");
    }

    [Fact]
    public void ActualizarObservacion_CuandoTextoEsNulo_DebePonerObservacionEnNulo()
    {
        // Arrange
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, "Observación original");

        // Act
        calificacion.ActualizarObservacion(null);

        // Assert
        calificacion.Observacion.Should().BeNull();
    }

    [Fact]
    public void ActualizarObservacion_CuandoTextoEsSoloEspacios_DebePonerObservacionEnNulo()
    {
        // Arrange
        var calificacion = CalificacionColaborador.Calificar(
            SolicitudIdValida, ClienteIdValido, ColaboradorIdValido,
            PuntajeValido, "Observación original");

        // Act
        calificacion.ActualizarObservacion("   ");

        // Assert
        calificacion.Observacion.Should().BeNull();
    }
}