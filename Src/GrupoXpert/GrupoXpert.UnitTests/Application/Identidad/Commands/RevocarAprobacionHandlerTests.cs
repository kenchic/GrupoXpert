using FluentAssertions;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Application.Identidad.Commands;
using GrupoXpert.Domain.Exceptions;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Identidad.Commands;

/// <summary>
/// Pruebas de la capa de Aplicación para el comando RevocarAprobacion.
/// </summary>
public sealed class RevocarAprobacionHandlerTests
{
    private readonly IUsuarioRepository _repositorio = Substitute.For<IUsuarioRepository>();
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();

    private RevocarAprobacionHandler CrearHandler() =>
        new(_repositorio, _unidadDeTrabajo);

    private static Usuario CrearAsesorAprobado()
    {
        var asesor = Usuario.Crear(
            "asesor@test.com", "hash_seguro", "Asesor Prueba",
            "token-activacion-123", tipo: TipoUsuario.Asesor);
        asesor.AprobarCuenta(Guid.NewGuid());
        return asesor;
    }

    [Fact]
    public async Task Handle_CuandoColaboradorAprobado_DebeRevocarYGuardar()
    {
        // Arrange
        var asesor = CrearAsesorAprobado();
        var comando = new RevocarAprobacionCommand(asesor.Id, Guid.NewGuid());

        _repositorio.ObtenerPorIdAsync(asesor.Id, Arg.Any<CancellationToken>())
            .Returns(asesor);

        var handler = CrearHandler();

        // Act
        await handler.Handle(comando, CancellationToken.None);

        // Assert
        asesor.EstaAprobado.Should().BeFalse();
        asesor.EstaActivo.Should().BeFalse();

        await _repositorio.Received(1).ActualizarAsync(asesor, Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoColaboradorNoExiste_DebeLanzarExcepcionDominio()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var comando = new RevocarAprobacionCommand(idInexistente, Guid.NewGuid());

        _repositorio.ObtenerPorIdAsync(idInexistente, Arg.Any<CancellationToken>())
            .Returns((Usuario?)null);

        var handler = CrearHandler();

        // Act
        var accion = () => handler.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("Colaborador no encontrado.");

        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoColaboradorNoPendiente_DebePropalarExcepcionDominio()
    {
        // Arrange — un asesor que NO fue aprobado aún (no se puede revocar)
        var asesorPendiente = Usuario.Crear(
            "asesor2@test.com", "hash_seguro", "Asesor Sin Aprobar",
            "token-456", tipo: TipoUsuario.Asesor);

        var comando = new RevocarAprobacionCommand(asesorPendiente.Id, Guid.NewGuid());

        _repositorio.ObtenerPorIdAsync(asesorPendiente.Id, Arg.Any<CancellationToken>())
            .Returns(asesorPendiente);

        var handler = CrearHandler();

        // Act
        var accion = () => handler.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ExcepcionDominio>()
            .WithMessage("La cuenta del colaborador no está aprobada.");

        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }
}
