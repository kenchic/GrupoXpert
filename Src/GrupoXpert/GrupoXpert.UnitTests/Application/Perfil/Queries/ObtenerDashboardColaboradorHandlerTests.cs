using FluentAssertions;
using GrupoXpert.Application.Perfil.Dtos;
using GrupoXpert.Application.Perfil.Queries;
using GrupoXpert.Domain.Perfil;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Perfil.Queries;

public sealed class ObtenerDashboardColaboradorHandlerTests
{
    private readonly IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IDashboardColaboradorRepository _dashboardRepositorio;
    private readonly ObtenerDashboardColaboradorHandler _manejador;

    private static readonly Guid UsuarioId = Guid.NewGuid();
    private static readonly Guid PerfilColaboradorId = Guid.NewGuid();
    private static readonly Guid SolicitudId = Guid.NewGuid();

    public ObtenerDashboardColaboradorHandlerTests()
    {
        _perfilColaboradorRepositorio = Substitute.For<IPerfilColaboradorRepository>();
        _dashboardRepositorio = Substitute.For<IDashboardColaboradorRepository>();

        _manejador = new ObtenerDashboardColaboradorHandler(
            _perfilColaboradorRepositorio, _dashboardRepositorio);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CASO EXITOSO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Handle_CuandoPerfilExiste_DebeRetornarDashboardCompleto()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);
        var reputacion = new ReputacionAcademica(4.2m, 10);
        var resumen = new ResumenActividadColaborador(3, 12, 5, 2, reputacion);
        var detalle = new DetalleSolicitudAsesor(
            SolicitudId, "Tesis", "Derecho Penal", "EnProceso",
            DateTime.UtcNow.AddDays(15), false, 2, null);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(resumen);
        _dashboardRepositorio.ObtenerSolicitudesPorAsesorAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(new List<DetalleSolicitudAsesor> { detalle });

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.PerfilColaboradorId.Should().Be(perfil.Id);
        resultado.UsuarioId.Should().Be(UsuarioId);
        resultado.ProyectosEnCurso.Should().Be(3);
        resultado.EntregasRealizadas.Should().Be(12);
        resultado.SolicitudesAbiertas.Should().Be(5);
        resultado.PostulacionesPendientes.Should().Be(2);
        resultado.Reputacion.PuntajePromedio.Should().Be(4.2m);
        resultado.Reputacion.TotalCalificaciones.Should().Be(10);
        resultado.Reputacion.Nivel.Should().Be("Avanzado");
        resultado.Solicitudes.Should().HaveCount(1);
        resultado.Solicitudes[0].AreaTematica.Should().Be("Derecho Penal");
        resultado.Solicitudes[0].TipoTrabajo.Should().Be("Tesis");
    }

    [Fact]
    public async Task Handle_CuandoNoHaySolicitudes_DebeRetornarListaVacia()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);
        var reputacion = ReputacionAcademica.SinCalificaciones;
        var resumen = new ResumenActividadColaborador(0, 0, 0, 0, reputacion);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(resumen);
        _dashboardRepositorio.ObtenerSolicitudesPorAsesorAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DetalleSolicitudAsesor>());

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Solicitudes.Should().BeEmpty();
        resultado.ProyectosEnCurso.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CuandoSolicitudUrgente_DebeReflejarPropiedadEnDto()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);
        var reputacion = new ReputacionAcademica(3.0m, 3);
        var resumen = new ResumenActividadColaborador(1, 5, 2, 0, reputacion);
        var detalleUrgente = new DetalleSolicitudAsesor(
            SolicitudId, "Ensayo", "Literatura", "Asignada",
            DateTime.UtcNow.AddDays(1), true, 0, null);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(resumen);
        _dashboardRepositorio.ObtenerSolicitudesPorAsesorAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(new List<DetalleSolicitudAsesor> { detalleUrgente });

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Solicitudes[0].EsUrgente.Should().BeTrue();
        resultado.Solicitudes[0].Estado.Should().Be("Asignada");
    }

    [Fact]
    public async Task Handle_CuandoTieneCalificacion_DebeIncluirPuntaje()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);
        var reputacion = new ReputacionAcademica(4.0m, 5);
        var resumen = new ResumenActividadColaborador(2, 8, 3, 0, reputacion);
        var detalle = new DetalleSolicitudAsesor(
            SolicitudId, "Monografia", "Administración", "Completada",
            DateTime.UtcNow.AddDays(-5), false, 3, 5);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(resumen);
        _dashboardRepositorio.ObtenerSolicitudesPorAsesorAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(new List<DetalleSolicitudAsesor> { detalle });

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Solicitudes[0].PuntajeCalificacion.Should().Be(5);
        resultado.Solicitudes[0].NumeroEntregas.Should().Be(3);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CASOS BORDE
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Handle_CuandoPerfilNoExiste_DebeRetornarNulo()
    {
        // Arrange
        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns((PerfilColaborador?)null);

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CuandoResumenNoExiste_DebeRetornarNulo()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns((ResumenActividadColaborador?)null);

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CuandoPerfilEsNuevoSinActividad_DebeRetornarDashboardEnCeros()
    {
        // Arrange
        var perfil = PerfilColaborador.Crear(UsuarioId);
        var reputacion = ReputacionAcademica.SinCalificaciones;
        var resumen = new ResumenActividadColaborador(0, 0, 0, 0, reputacion);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _dashboardRepositorio.ObtenerResumenAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(resumen);
        _dashboardRepositorio.ObtenerSolicitudesPorAsesorAsync(perfil.Id, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<DetalleSolicitudAsesor>());

        var query = new ObtenerDashboardColaboradorQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.ProyectosEnCurso.Should().Be(0);
        resultado.EntregasRealizadas.Should().Be(0);
        resultado.SolicitudesAbiertas.Should().Be(0);
        resultado.PostulacionesPendientes.Should().Be(0);
        resultado.Reputacion.Nivel.Should().Be("SinCalificar");
        resultado.Solicitudes.Should().BeEmpty();
    }
}