using FluentAssertions;
using GrupoXpert.Application.Calificacion.Commands;
using GrupoXpert.Application.Calificacion.Dtos;
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

namespace GrupoXpert.UnitTests.Application.Calificacion.Commands;

/// <summary>
/// Pruebas del handler CalificarColaboradorHandler (capa de Aplicación).
/// Validan la orquestación: solicitud debe existir, estar en Liberación,
/// pertenecer al cliente, no estar previamente calificada.
/// Se usan Mocks (NSubstitute) para todas las dependencias de infraestructura.
/// </summary>
public sealed class CalificarColaboradorHandlerTests
{
    // ── Mocks de dependencias ────────────────────────────────────────────────
    private readonly ICalificacionColaboradorRepository _calificacionRepositorio;
    private readonly ISolicitudAcademicaRepository _solicitudRepositorio;
    private readonly IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly CalificarColaboradorHandler _manejador;

    // ── Constantes de prueba ─────────────────────────────────────────────────
    private static readonly Guid SolicitudId = Guid.NewGuid();
    private static readonly Guid ClienteId = Guid.NewGuid();
    private static readonly Guid ColaboradorId = Guid.NewGuid();
    private static readonly Guid UsuarioColaboradorId = Guid.NewGuid();

    public CalificarColaboradorHandlerTests()
    {
        _calificacionRepositorio = Substitute.For<ICalificacionColaboradorRepository>();
        _solicitudRepositorio = Substitute.For<ISolicitudAcademicaRepository>();
        _perfilColaboradorRepositorio = Substitute.For<IPerfilColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();

        _manejador = new CalificarColaboradorHandler(
            _calificacionRepositorio, _solicitudRepositorio,
            _perfilColaboradorRepositorio, _usuarioRepositorio, _unidadDeTrabajo);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // FLUJO EXITOSO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Calificar_CuandoDatosSonValidos_DebeRetornarDtoConNombreColaborador()
    {
        // Arrange
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 5, "Excelente trabajo");
        var solicitud = CrearSolicitudEnLiberacion();

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitud);
        _calificacionRepositorio.ObtenerPorSolicitudAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns((CalificacionColaborador?)null);
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(ColaboradorId, Arg.Any<CancellationToken>()).Returns(CrearPerfilColaborador());
        _usuarioRepositorio.ObtenerPorIdAsync(UsuarioColaboradorId, Arg.Any<CancellationToken>()).Returns(CrearUsuarioColaborador());

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NombreColaborador.Should().Be("Carlos Asesor");
        resultado.Puntaje.Should().Be(5);
        resultado.Observacion.Should().Be("Excelente trabajo");
        resultado.SolicitudId.Should().Be(SolicitudId);
    }

    [Fact]
    public async Task Calificar_CuandoDatosSonValidos_DebePersistirCalificacion()
    {
        // Arrange
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 4, null);
        var solicitud = CrearSolicitudEnLiberacion();

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitud);
        _calificacionRepositorio.ObtenerPorSolicitudAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns((CalificacionColaborador?)null);
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(ColaboradorId, Arg.Any<CancellationToken>()).Returns(CrearPerfilColaborador());
        _usuarioRepositorio.ObtenerPorIdAsync(UsuarioColaboradorId, Arg.Any<CancellationToken>()).Returns(CrearUsuarioColaborador());

        // Act
        await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await _calificacionRepositorio.Received(1).AgregarAsync(Arg.Any<CalificacionColaborador>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Calificar_CuandoPerfilColaboradorNoExiste_DebeRetornarDtoSinNombre()
    {
        // Arrange
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 3, null);
        var solicitud = CrearSolicitudEnLiberacion();

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitud);
        _calificacionRepositorio.ObtenerPorSolicitudAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns((CalificacionColaborador?)null);
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(ColaboradorId, Arg.Any<CancellationToken>()).Returns((PerfilColaborador?)null);

        // Act
        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.NombreColaborador.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES DE SOLICITUD
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Calificar_CuandoSolicitudNoExiste_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 4, null);
        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns((SolicitudAcademica?)null);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*solicitud no existe*");
    }

    [Fact]
    public async Task Calificar_CuandoSolicitudEstaPendiente_DebeLanzarInvalidOperationException()
    {
        // Arrange — solicitud recién creada está en Pendiente
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 4, null);
        var solicitudPendiente = SolicitudAcademica.Crear(
            ClienteId, NivelAcademicoEnum.Pregrado, TipoTrabajoEnum.Tesis,
            "Tesis de prueba", DateTime.UtcNow.AddDays(30),
            50, NormaCitacionEnum.APA, IdiomaRequeridoEnum.Espanol,
            "Formato PDF", "Material base", false, false);

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitudPendiente);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Liberación*");
    }

    [Fact]
    public async Task Calificar_CuandoSolicitudEstaAsignada_DebeLanzarInvalidOperationException()
    {
        // Arrange — Pasar por Asignada
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 4, null);
        var solicitudAsignada = SolicitudAcademica.Crear(
            ClienteId, NivelAcademicoEnum.Pregrado, TipoTrabajoEnum.Tesis,
            "Tesis de prueba", DateTime.UtcNow.AddDays(30),
            50, NormaCitacionEnum.APA, IdiomaRequeridoEnum.Espanol,
            "Formato PDF", "Material base", false, false);
        solicitudAsignada.AsignarAsesor(Guid.NewGuid());

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitudAsignada);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Liberación*");
    }

    [Fact]
    public async Task Calificar_CuandoSolicitudEstaCancelada_DebeLanzarInvalidOperationException()
    {
        // Arrange — Cancelada se puede alcanzar desde Pendiente
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 4, null);
        var solicitudCancelada = SolicitudAcademica.Crear(
            ClienteId, NivelAcademicoEnum.Pregrado, TipoTrabajoEnum.Tesis,
            "Tesis de prueba", DateTime.UtcNow.AddDays(30),
            50, NormaCitacionEnum.APA, IdiomaRequeridoEnum.Espanol,
            "Formato PDF", "Material base", false, false);
        solicitudCancelada.Cancelar();

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitudCancelada);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Liberación*");
    }

    [Fact]
    public async Task Calificar_CuandoClienteNoEsElDueñoDeLaSolicitud_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var otroClienteId = Guid.NewGuid();
        var comando = new CalificarColaboradorCommand(SolicitudId, otroClienteId, ColaboradorId, 4, null);
        var solicitud = CrearSolicitudEnLiberacion();

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitud);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Solo el cliente*");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIÓN DE CALIFICACIÓN DUPLICADA
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Calificar_CuandoLaSolicitudYaTieneCalificacion_DebeLanzarInvalidOperationException()
    {
        // Arrange
        var comando = new CalificarColaboradorCommand(SolicitudId, ClienteId, ColaboradorId, 3, null);
        var solicitud = CrearSolicitudEnLiberacion();
        var calificacionExistente = CalificacionColaborador.Calificar(SolicitudId, ClienteId, ColaboradorId, 4, null);

        _solicitudRepositorio.ObtenerPorIdAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(solicitud);
        _calificacionRepositorio.ObtenerPorSolicitudAsync(SolicitudId, Arg.Any<CancellationToken>()).Returns(calificacionExistente);

        // Act
        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ya tiene una calificación*");
    }

    // ── Métodos auxiliares de prueba ──────────────────────────────────────────

    private static SolicitudAcademica CrearSolicitudEnLiberacion()
    {
        var solicitud = SolicitudAcademica.Crear(
            ClienteId, NivelAcademicoEnum.Pregrado, TipoTrabajoEnum.Tesis,
            "Inteligencia Artificial", DateTime.UtcNow.AddDays(30),
            50, NormaCitacionEnum.APA, IdiomaRequeridoEnum.Espanol,
            "Formato PDF", "Material base", false, false);

        var asesorPerfilId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorPerfilId);
        solicitud.Liberar();

        return solicitud;
    }

    private static PerfilColaborador CrearPerfilColaborador()
    {
        return PerfilColaborador.Crear(UsuarioColaboradorId);
    }

private static Usuario CrearUsuarioColaborador()
    {
        return Usuario.Crear("asesor@test.com", "hash123", "Carlos Asesor",
            "token-activacion", tipo: TipoUsuario.Asesor);
    }
}