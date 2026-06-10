using FluentAssertions;
using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Calificacion.Queries;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Calificacion.Queries;

/// <summary>
/// Pruebas del handler ObtenerMisCalificaciones.
/// Validan que se resuelva correctamente el PerfilColaborador
/// desde el UsuarioId y se retornen las calificaciones asociadas.
/// </summary>
public sealed class ObtenerMisCalificacionesHandlerTests
{
    private readonly IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly ICalificacionColaboradorRepository _calificacionRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerMisCalificacionesHandler _manejador;

    private static readonly Guid UsuarioId = Guid.NewGuid();
    private static readonly Guid PerfilColaboradorId = Guid.NewGuid();
    private static readonly Guid SolicitudId = Guid.NewGuid();
    private static readonly Guid ClienteId = Guid.NewGuid();

    public ObtenerMisCalificacionesHandlerTests()
    {
        _perfilColaboradorRepositorio = Substitute.For<IPerfilColaboradorRepository>();
        _calificacionRepositorio = Substitute.For<ICalificacionColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();

        _manejador = new ObtenerMisCalificacionesHandler(
            _perfilColaboradorRepositorio, _calificacionRepositorio, _usuarioRepositorio);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CASO EXITOSO
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ObtenerMisCalificaciones_CuandoAsesorTieneCalificaciones_DebeRetornarListaConNombre()
    {
        // Arrange
        var perfilColaborador = PerfilColaborador.Crear(UsuarioId);
        var calificacion = CalificacionColaborador.Calificar(SolicitudId, ClienteId, PerfilColaboradorId, 5, "Excelente");
        var usuarioAsesor = Usuario.Crear("asesor@test.com", "hash", "María Asesora", "token-activacion", tipo: TipoUsuario.Asesor);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns(perfilColaborador);
        _calificacionRepositorio.ObtenerPorColaboradorAsync(perfilColaborador.Id, Arg.Any<CancellationToken>()).Returns(new List<CalificacionColaborador> { calificacion });
        _usuarioRepositorio.ObtenerPorIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns(usuarioAsesor);

        var query = new ObtenerMisCalificacionesQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].NombreColaborador.Should().Be("María Asesora");
        resultado[0].Puntaje.Should().Be(5);
    }

    [Fact]
    public async Task ObtenerMisCalificaciones_CuandoAsesorNoTieneCalificaciones_DebeRetornarListaVacia()
    {
        // Arrange
        var perfilColaborador = PerfilColaborador.Crear(UsuarioId);

        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns(perfilColaborador);
        _calificacionRepositorio.ObtenerPorColaboradorAsync(perfilColaborador.Id, Arg.Any<CancellationToken>()).Returns(new List<CalificacionColaborador>());

        var query = new ObtenerMisCalificacionesQuery(UsuarioId);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeEmpty();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // VALIDACIONES
    // ═══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task ObtenerMisCalificaciones_CuandoPerfilColaboradorNoExiste_DebeLanzarInvalidOperationException()
    {
        // Arrange
        _perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(UsuarioId, Arg.Any<CancellationToken>()).Returns((PerfilColaborador?)null);

        var query = new ObtenerMisCalificacionesQuery(UsuarioId);

        // Act
        var accion = () => _manejador.Handle(query, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*perfil de colaborador*");
    }
}