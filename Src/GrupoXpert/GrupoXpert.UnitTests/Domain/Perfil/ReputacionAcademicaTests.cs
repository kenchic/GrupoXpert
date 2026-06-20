using FluentAssertions;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Perfil;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Perfil;

public sealed class ReputacionAcademicaTests
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN VÁLIDA
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(0.0, 0, NivelReputacion.SinCalificar)]
    [InlineData(2.0, 1, NivelReputacion.Principiante)]
    [InlineData(2.5, 1, NivelReputacion.Intermedio)]
    [InlineData(3.0, 3, NivelReputacion.Intermedio)]
    [InlineData(3.5, 5, NivelReputacion.Avanzado)]
    [InlineData(4.0, 10, NivelReputacion.Avanzado)]
    [InlineData(4.5, 20, NivelReputacion.Experto)]
    [InlineData(5.0, 50, NivelReputacion.Experto)]
    public void Crear_CuandoPuntajeEsValido_DebeAsignarNivelCorrecto(decimal puntaje, int total, NivelReputacion nivelEsperado)
    {
        // Act
        var reputacion = new ReputacionAcademica(puntaje, total);

        // Assert
        reputacion.PuntajePromedio.Should().Be(puntaje);
        reputacion.TotalCalificaciones.Should().Be(total);
        reputacion.Nivel.Should().Be(nivelEsperado);
    }

    [Fact]
    public void Crear_CuandoUsarMetodoFabricaSinCalificaciones_DebeRetornarValoresPorDefecto()
    {
        // Arrange & Act
        var reputacion = ReputacionAcademica.SinCalificaciones;

        // Assert
        reputacion.PuntajePromedio.Should().Be(0m);
        reputacion.TotalCalificaciones.Should().Be(0);
        reputacion.Nivel.Should().Be(NivelReputacion.SinCalificar);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE INVARIANTES
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Crear_CuandoPuntajePromedioEsNegativo_DebeLanzarExcepcionDominio(decimal puntajeInvalido)
    {
        // Act
        var accion = () => new ReputacionAcademica(puntajeInvalido, 0);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*puntaje*reputación*0*5*");
    }

    [Theory]
    [InlineData(5.01)]
    [InlineData(6)]
    [InlineData(10)]
    public void Crear_CuandoPuntajePromedioEsMayorQueCinco_DebeLanzarExcepcionDominio(decimal puntajeInvalido)
    {
        // Act
        var accion = () => new ReputacionAcademica(puntajeInvalido, 1);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*puntaje*reputación*0*5*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoTotalCalificacionesEsNegativo_DebeLanzarExcepcionDominio(int totalInvalido)
    {
        // Act
        var accion = () => new ReputacionAcademica(3m, totalInvalido);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*total de calificaciones*negativo*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RECALCULAR REPUTACIÓN
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Recalcular_ConListaDePuntajes_DebeCalcularPromedioCorrecto()
    {
        // Arrange
        var reputacion = new ReputacionAcademica(0m, 0);
        var puntajes = new[] { 4, 5, 3, 5, 4 };

        // Act
        var nueva = reputacion.Recalcular(puntajes);

        // Assert
        nueva.PuntajePromedio.Should().Be(4.2m);
        nueva.TotalCalificaciones.Should().Be(5);
        nueva.Nivel.Should().Be(NivelReputacion.Avanzado);
    }

    [Fact]
    public void Recalcular_ConListaVacia_DebeRetornarSinCalificaciones()
    {
        // Arrange
        var reputacion = new ReputacionAcademica(4.5m, 10);
        var puntajes = Array.Empty<int>();

        // Act
        var nueva = reputacion.Recalcular(puntajes);

        // Assert
        nueva.PuntajePromedio.Should().Be(0m);
        nueva.TotalCalificaciones.Should().Be(0);
        nueva.Nivel.Should().Be(NivelReputacion.SinCalificar);
    }

    [Fact]
    public void Recalcular_CuandoTodasSonCincoEstrellas_DebeSerExperto()
    {
        // Arrange
        var reputacion = ReputacionAcademica.SinCalificaciones;
        var puntajes = new[] { 5, 5, 5, 5, 5 };

        // Act
        var nueva = reputacion.Recalcular(puntajes);

        // Assert
        nueva.PuntajePromedio.Should().Be(5.0m);
        nueva.Nivel.Should().Be(NivelReputacion.Experto);
    }

    [Fact]
    public void Recalcular_CuandoPromedioEsBajo_DebeSerPrincipiante()
    {
        // Arrange
        var reputacion = ReputacionAcademica.SinCalificaciones;
        var puntajes = new[] { 1, 2, 1, 2 };

        // Act
        var nueva = reputacion.Recalcular(puntajes);

        // Assert
        nueva.PuntajePromedio.Should().Be(1.5m);
        nueva.Nivel.Should().Be(NivelReputacion.Principiante);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // IGUALDAD ESTRUCTURAL (Value Object)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Igualdad_CuandoDosReputacionesTienenMismosValores_DebeSerIguales()
    {
        // Arrange
        var reputacion1 = new ReputacionAcademica(4.2m, 5);
        var reputacion2 = new ReputacionAcademica(4.2m, 5);

        // Assert
        reputacion1.Should().Be(reputacion2);
        reputacion1.GetHashCode().Should().Be(reputacion2.GetHashCode());
    }

    [Fact]
    public void Igualdad_CuandoPuntajeEsDistinto_DebeSerDistintas()
    {
        // Arrange
        var reputacion1 = new ReputacionAcademica(4.0m, 5);
        var reputacion2 = new ReputacionAcademica(3.0m, 5);

        // Assert
        reputacion1.Should().NotBe(reputacion2);
    }

    [Fact]
    public void Igualdad_CuandoTotalCalificacionesEsDistinto_DebeSerDistintas()
    {
        // Arrange
        var reputacion1 = new ReputacionAcademica(4.0m, 5);
        var reputacion2 = new ReputacionAcademica(4.0m, 10);

        // Assert
        reputacion1.Should().NotBe(reputacion2);
    }
}