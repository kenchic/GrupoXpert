using System;
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

public class ValidarPerfilProfesionalHandlerTests
{
    private readonly IPerfilColaboradorRepository _repositorioPerfil;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly ValidarPerfilProfesionalHandler _manejador;

    public ValidarPerfilProfesionalHandlerTests()
    {
        _repositorioPerfil = Substitute.For<IPerfilColaboradorRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new ValidarPerfilProfesionalHandler(_repositorioPerfil, _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoPerfilExiste_DebeValidarPerfilYEstablecerEvaluacion()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var perfil = PerfilColaborador.Crear(usuarioId);
        var comando = new ValidarPerfilProfesionalCommand(usuarioId, 4.8m);

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().Be(Unit.Value);
        perfil.ValidadoPorAdmin.Should().BeTrue();
        perfil.EvaluacionCalidad.Should().Be(4.8m);

        await _repositorioPerfil.Received(1).ActualizarAsync(perfil, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoPerfilNoExiste_DebeLanzarExcepcion()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var comando = new ValidarPerfilProfesionalCommand(usuarioId, 4.0m);

        _repositorioPerfil.ObtenerPorUsuarioIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns((PerfilColaborador)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("El perfil del colaborador no existe.");
    }
}
