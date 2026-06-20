using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Domain.Perfil.Events;
using Xunit;

namespace GrupoXpert.UnitTests.Domain;

public class PerfilColaboradorTests
{
    [Fact]
    public void Crear_ConUsuarioId_DebeInicializarConValoresPorDefecto()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var perfil = PerfilColaborador.Crear(usuarioId);

        // Assert
        perfil.UsuarioId.Should().Be(usuarioId);
        perfil.ValidadoPorAdmin.Should().BeFalse();
        perfil.NivelAcademico.Should().Be(NivelAcademico.NoEspecificado);
        perfil.DisponibilidadHorasSemana.Should().Be(0);
        perfil.CargaAcademicaIdeal.Should().Be(0);
        perfil.EvaluacionCalidad.Should().Be(0m);
        perfil.AreasConocimiento.Should().BeEmpty();
        perfil.TiposTrabajo.Should().BeEmpty();
        perfil.Idiomas.Should().BeEmpty();
        perfil.NormasCitacion.Should().BeEmpty();
    }

    [Fact]
    public void ActualizarPerfil_ConDatosValidos_DebeActualizarYRegistrarEvento()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var nivel = NivelAcademico.Maestria;
        var disponibilidad = 20;
        var cargaIdeal = 5;

        // Act
        perfil.ActualizarPerfil(nivel, disponibilidad, cargaIdeal);

        // Assert
        perfil.NivelAcademico.Should().Be(nivel);
        perfil.DisponibilidadHorasSemana.Should().Be(disponibilidad);
        perfil.CargaAcademicaIdeal.Should().Be(cargaIdeal);
        perfil.EventosDominio.Should().ContainItemsAssignableTo<PerfilColaboradorActualizadoEvent>();
    }

    [Fact]
    public void ActualizarPerfil_ConDisponibilidadNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());

        // Act
        var accion = () => perfil.ActualizarPerfil(NivelAcademico.Pregrado, -5, 2);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*disponibilidad*");
    }

    [Fact]
    public void ActualizarPerfil_ConCargaIdealMayorADisponibilidad_DebeLanzarExcepcion()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());

        // Act
        var accion = () => perfil.ActualizarPerfil(NivelAcademico.Pregrado, 10, 15);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*carga ideal*");
    }

    [Fact]
    public void ValidarPerfil_CuandoNoEstaValidado_DebeMarcarComoValidadoYRegistrarEvento()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());

        // Act
        perfil.ValidarPerfil();

        // Assert
        perfil.ValidadoPorAdmin.Should().BeTrue();
        perfil.EventosDominio.Should().ContainItemsAssignableTo<PerfilColaboradorValidadoEvent>();
    }

    [Fact]
    public void ActualizarEvaluacionCalidad_ConValorValido_DebeActualizar()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var evaluacion = 4.5m;

        // Act
        perfil.ActualizarEvaluacionCalidad(evaluacion);

        // Assert
        perfil.EvaluacionCalidad.Should().Be(evaluacion);
    }

    [Fact]
    public void ActualizarEvaluacionCalidad_ConValorInvalido_DebeLanzarExcepcion()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());

        // Act
        var accion = () => perfil.ActualizarEvaluacionCalidad(6.0m);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*evaluación*");
    }

    [Fact]
    public void AgregarAreaConocimiento_CuandoNoExiste_DebeAgregar()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var area = "Ingeniería de Software";

        // Act
        perfil.AgregarAreaConocimiento(area);

        // Assert
        perfil.AreasConocimiento.Should().Contain(area);
        perfil.AreasConocimiento.Should().HaveCount(1);
    }

    [Fact]
    public void AgregarAreaConocimiento_CuandoYaExiste_NoDebeDuplicar()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var area = "Ingeniería de Software";
        perfil.AgregarAreaConocimiento(area);

        // Act
        perfil.AgregarAreaConocimiento(area);

        // Assert
        perfil.AreasConocimiento.Should().HaveCount(1);
    }

    [Fact]
    public void RemoverAreaConocimiento_CuandoExiste_DebeRemover()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var area = "Ingeniería de Software";
        perfil.AgregarAreaConocimiento(area);

        // Act
        perfil.RemoverAreaConocimiento(area);

        // Assert
        perfil.AreasConocimiento.Should().NotContain(area);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ACTUALIZAR REPUTACIÓN
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public void ActualizarReputacion_ConReputacionValida_DebeActualizarYRegistrarEvento()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var nuevaReputacion = new ReputacionAcademica(4.5m, 15);

        // Act
        perfil.ActualizarReputacion(nuevaReputacion);

        // Assert
        perfil.Reputacion.Should().Be(nuevaReputacion);
        perfil.Reputacion.Nivel.Should().Be(NivelReputacion.Experto);
        perfil.EventosDominio.Should().ContainItemsAssignableTo<ReputacionAcademicaActualizadaEvent>();
    }

    [Fact]
    public void Crear_CuandoPerfilEsNuevo_DebeTenerReputacionSinCalificaciones()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();

        // Act
        var perfil = PerfilColaborador.Crear(usuarioId);

        // Assert
        perfil.Reputacion.Should().Be(ReputacionAcademica.SinCalificaciones);
        perfil.Reputacion.PuntajePromedio.Should().Be(0m);
        perfil.Reputacion.TotalCalificaciones.Should().Be(0);
        perfil.Reputacion.Nivel.Should().Be(NivelReputacion.SinCalificar);
    }

    [Fact]
    public void ActualizarReputacion_CuandoReputacionEsNula_DebeLanzarExcepcion()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());

        // Act
        var accion = () => perfil.ActualizarReputacion(null!);

        // Assert
        accion.Should().Throw<ArgumentException>().WithMessage("*reputación*nula*");
    }

    [Fact]
    public void ActualizarReputacion_EventoDebeContenerDatosCorrectos()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var nuevaReputacion = new ReputacionAcademica(3.8m, 7);

        // Act
        perfil.ActualizarReputacion(nuevaReputacion);

        // Assert
        var evento = perfil.EventosDominio.OfType<ReputacionAcademicaActualizadaEvent>().Single();
        evento.PerfilColaboradorId.Should().Be(perfil.Id);
        evento.PuntajePromedio.Should().Be(3.8m);
        evento.TotalCalificaciones.Should().Be(7);
        evento.NivelReputacion.Should().Be("Avanzado");
    }

    [Fact]
    public void Reputacion_AlRecalcular_DebeReflejarNuevaCalificacion()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(Guid.NewGuid());
        var puntajes = new[] { 5, 5, 4, 5 };
        var reputacionCalculada = perfil.Reputacion.Recalcular(puntajes);

        // Act
        perfil.ActualizarReputacion(reputacionCalculada);

        // Assert
        perfil.Reputacion.PuntajePromedio.Should().Be(4.75m);
        perfil.Reputacion.Nivel.Should().Be(NivelReputacion.Experto);
    }
}
