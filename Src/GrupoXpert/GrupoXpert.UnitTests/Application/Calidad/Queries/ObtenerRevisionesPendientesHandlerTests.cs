using FluentAssertions;
using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Calidad.Queries;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Calidad.Queries;

public class ObtenerRevisionesPendientesHandlerTests
{
    private readonly IRevisionCalidadRepository _revisionRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerRevisionesPendientesHandler _manejador;

    public ObtenerRevisionesPendientesHandlerTests()
    {
        _revisionRepositorio = Substitute.For<IRevisionCalidadRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _manejador = new ObtenerRevisionesPendientesHandler(_revisionRepositorio, _usuarioRepositorio);
    }

    [Fact]
    public async Task Handle_CuandoHayRevisionesPendientes_DebeRetornarListaDto()
    {
        // Arrange
        var revision1 = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var revision2 = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var revisiones = new List<RevisionCalidad> { revision1, revision2 };
        var consulta = new ObtenerRevisionesPendientesQuery();

        _revisionRepositorio.ObtenerPendientesAsync(Arg.Any<CancellationToken>()).Returns(revisiones);
        _usuarioRepositorio.ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().HaveCount(2);
        resultado[0].Id.Should().Be(revision1.Id);
        resultado[1].Id.Should().Be(revision2.Id);
    }

    [Fact]
    public async Task Handle_CuandoNoHayRevisionesPendientes_DebeRetornarListaVacia()
    {
        // Arrange
        var consulta = new ObtenerRevisionesPendientesQuery();

        _revisionRepositorio.ObtenerPendientesAsync(Arg.Any<CancellationToken>()).Returns(
            new List<RevisionCalidad>());

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CuandoRevisoresExisten_DebeIncluirNombreEnDto()
    {
        // Arrange
        var revisorId = Guid.NewGuid();
        var revision = RevisionCalidad.Crear(Guid.NewGuid(), revisorId);
        var revisiones = new List<RevisionCalidad> { revision };
        var consulta = new ObtenerRevisionesPendientesQuery();
        var usuarioRevisor = Usuario.Crear(
            "revisor@test.com", "hash123", "Laura Revisora",
            "token-activacion-revisor", tipo: TipoUsuario.Revisor);

        _revisionRepositorio.ObtenerPendientesAsync(Arg.Any<CancellationToken>()).Returns(revisiones);
        _usuarioRepositorio.ObtenerPorIdAsync(revisorId, Arg.Any<CancellationToken>()).Returns(usuarioRevisor);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].NombreRevisor.Should().Be("Laura Revisora");
    }

    [Fact]
    public async Task Handle_CuandoHayRevisionDeCadaEstado_DebeIncluirTodas()
    {
        // Arrange
        var revisionPendiente = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        var revisionAprobada = RevisionCalidad.Crear(Guid.NewGuid(), Guid.NewGuid());
        revisionAprobada.OtorgarVistoBueno();

        var revisiones = new List<RevisionCalidad> { revisionPendiente, revisionAprobada };
        var consulta = new ObtenerRevisionesPendientesQuery();

        _revisionRepositorio.ObtenerPendientesAsync(Arg.Any<CancellationToken>()).Returns(revisiones);
        _usuarioRepositorio.ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        // Act
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(r => r.Estado == (int)EstadoRevisionCalidad.Pendiente);
        resultado.Should().Contain(r => r.Estado == (int)EstadoRevisionCalidad.Aprobado);
    }
}
