using FluentAssertions;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Perfil;
using Xunit;

namespace GrupoXpert.UnitTests.Domain.Perfil;

public sealed class ResumenActividadColaboradorTests
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CREACIÓN VÁLIDA
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Crear_ConDatosValidos_DebeAsignarTodosLosCampos()
    {
        // Arrange
        var proyectosEnCurso = 3;
        var entregasRealizadas = 12;
        var solicitudesAbiertas = 5;
        var postulacionesPendientes = 2;
        var reputacion = new ReputacionAcademica(4.2m, 8);

        // Act
        var resumen = new ResumenActividadColaborador(
            proyectosEnCurso,
            entregasRealizadas,
            solicitudesAbiertas,
            postulacionesPendientes,
            reputacion);

        // Assert
        resumen.ProyectosEnCurso.Should().Be(proyectosEnCurso);
        resumen.EntregasRealizadas.Should().Be(entregasRealizadas);
        resumen.SolicitudesAbiertas.Should().Be(solicitudesAbiertas);
        resumen.PostulacionesPendientes.Should().Be(postulacionesPendientes);
        resumen.Reputacion.Should().Be(reputacion);
    }

    [Fact]
    public void Crear_ConCeros_DebeCrearseCorrectamente()
    {
        // Act
        var resumen = new ResumenActividadColaborador(0, 0, 0, 0, ReputacionAcademica.SinCalificaciones);

        // Assert
        resumen.ProyectosEnCurso.Should().Be(0);
        resumen.EntregasRealizadas.Should().Be(0);
        resumen.SolicitudesAbiertas.Should().Be(0);
        resumen.PostulacionesPendientes.Should().Be(0);
        resumen.Reputacion.Nivel.Should().Be(NivelReputacion.SinCalificar);
    }

    [Fact]
    public void Crear_CuandoReputacionEsNula_DebeUsarSinCalificaciones()
    {
        // Act
        var resumen = new ResumenActividadColaborador(1, 2, 3, 4, null!);

        // Assert
        resumen.Reputacion.Should().Be(ReputacionAcademica.SinCalificaciones);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE INVARIANTES
    // ═══════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoProyectosEnCursoEsNegativo_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new ResumenActividadColaborador(
            valorInvalido, 0, 0, 0, ReputacionAcademica.SinCalificaciones);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*proyectos en curso*negativo*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoEntregasRealizadasEsNegativo_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new ResumenActividadColaborador(
            0, valorInvalido, 0, 0, ReputacionAcademica.SinCalificaciones);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*entregas realizadas*negativo*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoSolicitudesAbiertasEsNegativo_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new ResumenActividadColaborador(
            0, 0, valorInvalido, 0, ReputacionAcademica.SinCalificaciones);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*solicitudes abiertas*negativo*");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Crear_CuandoPostulacionesPendientesEsNegativo_DebeLanzarExcepcionDominio(int valorInvalido)
    {
        // Act
        var accion = () => new ResumenActividadColaborador(
            0, 0, 0, valorInvalido, ReputacionAcademica.SinCalificaciones);

        // Assert
        accion.Should().Throw<ExcepcionDominio>()
            .WithMessage("*postulaciones pendientes*negativo*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // IGUALDAD ESTRUCTURAL (Value Object)
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void Igualdad_CuandoDosResumenesTienenMismosValores_DebeSerIguales()
    {
        // Arrange
        var reputacion = new ReputacionAcademica(3.5m, 4);
        var resumen1 = new ResumenActividadColaborador(2, 8, 3, 1, reputacion);
        var resumen2 = new ResumenActividadColaborador(2, 8, 3, 1, reputacion);

        // Assert
        resumen1.Should().Be(resumen2);
        resumen1.GetHashCode().Should().Be(resumen2.GetHashCode());
    }

    [Fact]
    public void Igualdad_CuandoProyectosEnCursoEsDistinto_DebeSerDistintos()
    {
        // Arrange
        var reputacion = ReputacionAcademica.SinCalificaciones;
        var resumen1 = new ResumenActividadColaborador(2, 0, 0, 0, reputacion);
        var resumen2 = new ResumenActividadColaborador(3, 0, 0, 0, reputacion);

        // Assert
        resumen1.Should().NotBe(resumen2);
    }
}