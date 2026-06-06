using FluentAssertions;
using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Calidad.Queries;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Calidad.Queries;

public class ObtenerRevisionPorAvanceHandlerTests
{
    private readonly IRevisionCalidadRepository _revisionRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerRevisionPorAvanceHandler _manejador;

    public ObtenerRevisionPorAvanceHandlerTests()
    {
        _revisionRepositorio = Substitute.For<IRevisionCalidadRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _manejador = new ObtenerRevisionPorAvanceHandler(_revisionRepositorio, _usuarioRepositorio);
    }

    [Fact]
    public async Task Handle_CuandoRevisionExiste_DebeRetornarDto()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var revisorId = Guid.NewGuid();
        var revision = RevisionCalidad.Crear(avanceId, revisorId);
        var consulta = new ObtenerRevisionPorAvanceQuery(avanceId);
        var usuarioRevisor = Usuario.Crear(
            "revisor@test.com", "hash123", "Pedro Revisor",
            "token-activacion-revisor", tipo: TipoUsuario.Revisor);

        _revisionRepositorio.ObtenerPorAvanceAsync(avanceId, Arg.Any<CancellationToken>()).Returns(revision);
        _usuarioRepositorio.ObtenerPorIdAsync(revisorId, Arg.Any<CancellationToken>()).Returns(usuarioRevisor);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(revision.Id);
        resultado.AvanceId.Should().Be(avanceId);
        resultado.NombreRevisor.Should().Be("Pedro Revisor");
        resultado.Estado.Should().Be((int)EstadoRevisionCalidad.Pendiente);
    }

    [Fact]
    public async Task Handle_CuandoRevisionNoExiste_DebeRetornarNull()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var consulta = new ObtenerRevisionPorAvanceQuery(avanceId);

        _revisionRepositorio.ObtenerPorAvanceAsync(avanceId, Arg.Any<CancellationToken>()).Returns((RevisionCalidad?)null);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        await _usuarioRepositorio.DidNotReceive().ObtenerPorIdAsync(
            Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoRevisorNoExiste_DebeRetornarDtoSinNombre()
    {
        // Arrange
        var avanceId = Guid.NewGuid();
        var revision = RevisionCalidad.Crear(avanceId, Guid.NewGuid());
        var consulta = new ObtenerRevisionPorAvanceQuery(avanceId);

        _revisionRepositorio.ObtenerPorAvanceAsync(avanceId, Arg.Any<CancellationToken>()).Returns(revision);
        _usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.NombreRevisor.Should().BeNull();
    }
}
