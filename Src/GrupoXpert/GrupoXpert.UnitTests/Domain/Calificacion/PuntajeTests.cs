using FluentAssertions;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Exceptions;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Calificacion;

/// <summary>
/// Pruebas unitarias del Objeto de Valor Puntaje.
/// Validan el rango permitido (1-5) y la igualdad estructural.
/// </summary>
public sealed class PuntajeTests
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN VÁLIDA
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Crear_CuandoValorEstaEnRango_DebeCrearseCorrectamente(int valor)
    {
        // Act
        var puntaje = new Puntaje(valor);

        // Assert
        puntaje.Valor.Should().Be(valor);
    }

    [Fact]
    public void Crear_CuandoSeUsanConstantesEstaticas_DebeCrearseCorrectamente()
    {
        // Assert
        Puntaje.UnaEstrella.Valor.Should().Be(1);
        Puntaje.DosEstrellas.Valor.Should().Be(2);
        Puntaje.TresEstrellas.Valor.Should().Be(3);
        Puntaje.CuatroEstrellas.Valor.Should().Be(4);
        Puntaje.CincoEstrellas.Valor.Should().Be(5);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE RANGO
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoValorEsMenorQueUno_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new Puntaje(valorInvalido);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*puntaje*1*5*");
    }

    [Theory]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(100)]
    public void Crear_CuandoValorEsMayorQueCinco_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new Puntaje(valorInvalido);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*puntaje*1*5*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // IGUALDAD ESTRUCTURAL (Value Object)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Igualdad_CuandoDosPuntajesTienenMismoValor_DebeSerIguales()
    {
        // Arrange
        var puntaje1 = new Puntaje(3);
        var puntaje2 = new Puntaje(3);

        // Assert
        puntaje1.Should().Be(puntaje2);
        puntaje1.GetHashCode().Should().Be(puntaje2.GetHashCode());
    }

    [Fact]
    public void Igualdad_CuandoDosPuntajesTienenDistintoValor_DebeSerDistintos()
    {
        // Arrange
        var puntaje1 = new Puntaje(2);
        var puntaje2 = new Puntaje(4);

        // Assert
        puntaje1.Should().NotBe(puntaje2);
    }
}