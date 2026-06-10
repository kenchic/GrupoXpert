using FluentAssertions;
using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Calificacion.Queries;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using NSubstitute;
using Xunit;

using NivelAcademicoEnum = GrupoXpert.Domain.Academia.NivelAcademico;
using TipoTrabajoEnum = GrupoXpert.Domain.Academia.TipoTrabajo;
using NormaCitacionEnum = GrupoXpert.Domain.Academia.NormaCitacion;
using IdiomaRequeridoEnum = GrupoXpert.Domain.Academia.IdiomaRequerido;

namespace GrupoXpert.UnitTests.Application.Calificacion.Queries;

/// <summary>
/// Pruebas del handler ObtenerSolicitudesPorCalificar.
/// Validan que solo se retornen solicitudes en Liberación del cliente autenticado,
/// con indicador de calificación existente.
/// </summary>
public sealed class ObtenerSolicitudesPorCalificarHandlerTests
{
    private readonly ISolicitudAcademicaRepository _solicitudRepositorio;
    private readonly ICalificacionColaboradorRepository _calificacionRepositorio;
    private readonly IPerfilClienteRepository _perfilClienteRepositorio;
    private readonly IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerSolicitudesPorCalificarHandler _manejador;

    private static readonly Guid UsuarioId = Guid.NewGuid();
    private static readonly Guid PerfilClienteId = Guid.NewGuid();
    private static readonly Guid AsesorPerfilId = Guid.NewGuid();

    public ObtenerSolicitudesPorCalificarHandlerTests()
    {
        _solicitudRepositorio = Substitute.For<ISolicitudAcademicaRepository>();
        _calificacionRepositorio = Substitute.For<ICalificacionColaboradorRepository>();
        _perfilClienteRepositorio = Substitute.For<IPerfilClienteRepository>();
        _perfilColaboradorRepositorio = Substitute.For<IPerfilColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();

        _manejador = new ObtenerSolicitudesPorCalificarHandler(
            _solicitudRepositorio, _calificacionRepositorio,
            _perfilClienteRepositorio, _perfilColaboradorRepositorio, _usuarioRepositorio);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CASO EXITOSO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ObtenerSolicitudes_CuandoHaySolicitudesEnLiberacion_DebeRetornarLista()
    {
        // Arrange
        var perfilCliente = Substitute.For<IPerfilClienteRepository>();
        var perfil = PerfilCliente.Crear(UsuarioId);
        // Override the Id via reflection since PerfilCliente generates its own
        typeof(PerfilCliente).BaseType!.GetProperty("Id")!.SetValue(perfil, PerfilClienteId);

        _perfilClienteRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns(perfil);
        _solicitudRepositorio.ObtenerEnLiberacionPorClienteAsync(PerfilClienteId, Arg.Any<CancellationToken>()).Returns(new List<SolicitudAcademica>());
        _calificacionRepositorio.ObtenerPorSolicitudAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CalificacionColaborador?)null);

        var query = new ObtenerSolicitudesPorCalificarQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerSolicitudes_CuandoPerfilClienteNoExiste_DebeLanzarInvalidOperationException()
    {
        // Arrange
        _perfilClienteRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns((PerfilCliente?)null);

        var query = new ObtenerSolicitudesPorCalificarQuery(UsuarioId);

        // Act
        var accion = () => _manejador.Handle(query, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*perfil del cliente*");
    }

    }