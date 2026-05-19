using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Perfil.Commands;
using GrupoXpert.Domain.Perfil;
using MediatR;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application;

public class ActualizarPerfilColaboradorHandlerTests
{
    private readonly IPerfilColaboradorRepository _repositorioPerfil;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly ActualizarPerfilColaboradorHandler _manejador;

    public ActualizarPerfilColaboradorHandlerTests()
    {
        _repositorioPerfil = Substitute.For<IPerfilColaboradorRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new ActualizarPerfilColaboradorHandler(_repositorioPerfil, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoPerfilNoExiste_DebeCrearNuevoYGuardar()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var comando = new ActualizarPerfilColaboradorCommand(
            usuarioId,
            (int)NivelAcademico.Maestria,
            30,
            10,
            new List<string> { "Derecho", "Psicología" },
            new List<string> { "Ensayos" },
            new List<string> { "Español", "Inglés" },
            new List<string> { "APA 7" });

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns((PerfilColaborador)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        await _repositorioPerfil.Received(1).AgregarAsync(Arg.Any<PerfilColaborador>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoPerfilExiste_DebeActualizarPropiedadesYListasYGuardar()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var perfilExistente = PerfilColaborador.Crear(usuarioId);
        perfilExistente.AgregarAreaConocimiento("Medicina");
        perfilExistente.AgregarIdioma("Español");

        var comando = new ActualizarPerfilColaboradorCommand(
            usuarioId,
            (int)NivelAcademico.Doctorado,
            20,
            5,
            new List<string> { "Derecho" }, // Remueve Medicina, agrega Derecho
            new List<string> { "Tesis" },
            new List<string> { "Español", "Portugués" }, // Agrega Portugués
            new List<string> { "IEEE" });

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns(perfilExistente);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        perfilExistente.NivelAcademico.Should().Be(NivelAcademico.Doctorado);
        perfilExistente.DisponibilidadHorasSemana.Should().Be(20);
        perfilExistente.CargaAcademicaIdeal.Should().Be(5);
        
        perfilExistente.AreasConocimiento.Should().Contain("Derecho");
        perfilExistente.AreasConocimiento.Should().NotContain("Medicina");
        perfilExistente.Idiomas.Should().Contain("Español");
        perfilExistente.Idiomas.Should().Contain("Portugués");
        perfilExistente.TiposTrabajo.Should().Contain("Tesis");
        perfilExistente.NormasCitacion.Should().Contain("IEEE");

        await _repositorioPerfil.Received(1).ActualizarAsync(perfilExistente, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
