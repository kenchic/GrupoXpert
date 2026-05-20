using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GrupoXpert.Application.Academia.Queries;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

// Resolving namespace ambiguities
using NivelAcademico = GrupoXpert.Domain.Academia.NivelAcademico;
using TipoTrabajo = GrupoXpert.Domain.Academia.TipoTrabajo;
using NormaCitacion = GrupoXpert.Domain.Academia.NormaCitacion;

namespace GrupoXpert.UnitTests.Application.Academia.Queries;

public class ObtenerSolicitudPorIdHandlerTests
{
    private readonly ISolicitudAcademicaRepository _repositorio;
    private readonly IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerSolicitudPorIdHandler _manejador;

    public ObtenerSolicitudPorIdHandlerTests()
    {
        _repositorio = Substitute.For<ISolicitudAcademicaRepository>();
        _perfilColaboradorRepositorio = Substitute.For<IPerfilColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _manejador = new ObtenerSolicitudPorIdHandler(
            _repositorio,
            _perfilColaboradorRepositorio,
            _usuarioRepositorio
        );
    }

    [Fact]
    public async Task Handle_CuandoSolicitudNoExiste_DebeRetornarNull()
    {
        // Arrange
        var solicitudId = Guid.NewGuid();
        var query = new ObtenerSolicitudPorIdQuery(solicitudId);

        _repositorio.ObtenerPorIdAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns((SolicitudAcademica)null!);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().BeNull();
        await _perfilColaboradorRepositorio.DidNotReceive().ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _usuarioRepositorio.DidNotReceive().ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteSinAsesor_DebeRetornarDtoSinNombreAsesor()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId,
            NivelAcademico.Pregrado,
            TipoTrabajo.Tesis,
            "Derecho",
            DateTime.UtcNow.AddDays(5),
            10,
            NormaCitacion.APA,
            IdiomaRequerido.Espanol,
            "PDF",
            "Instrucciones",
            false,
            false
        );
        var query = new ObtenerSolicitudPorIdQuery(solicitud.Id);

        _repositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(solicitud.Id);
        resultado.ClienteId.Should().Be(clienteId);
        resultado.AsesorId.Should().BeNull();
        resultado.NombreAsesor.Should().BeNull();

        await _perfilColaboradorRepositorio.DidNotReceive().ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _usuarioRepositorio.DidNotReceive().ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteConAsesorPeroPerfilNoExiste_DebeRetornarDtoConNombreAsesorNull()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId,
            NivelAcademico.Pregrado,
            TipoTrabajo.Tesis,
            "Derecho",
            DateTime.UtcNow.AddDays(5),
            10,
            NormaCitacion.APA,
            IdiomaRequerido.Espanol,
            "PDF",
            "Instrucciones",
            false,
            false
        );
        var asesorId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorId);

        var query = new ObtenerSolicitudPorIdQuery(solicitud.Id);

        _repositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        _perfilColaboradorRepositorio.ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>())
            .Returns((PerfilColaborador)null!);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(solicitud.Id);
        resultado.AsesorId.Should().Be(asesorId);
        resultado.NombreAsesor.Should().BeNull();

        await _perfilColaboradorRepositorio.Received(1).ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>());
        await _usuarioRepositorio.DidNotReceive().ObtenerPorIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteConAsesorYPerfilExistePeroUsuarioNoExiste_DebeRetornarDtoConNombreAsesorNull()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId,
            NivelAcademico.Pregrado,
            TipoTrabajo.Tesis,
            "Derecho",
            DateTime.UtcNow.AddDays(5),
            10,
            NormaCitacion.APA,
            IdiomaRequerido.Espanol,
            "PDF",
            "Instrucciones",
            false,
            false
        );
        var asesorId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorId);

        var usuarioId = Guid.NewGuid();
        var perfilAsesor = PerfilColaborador.Crear(usuarioId);

        var query = new ObtenerSolicitudPorIdQuery(solicitud.Id);

        _repositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        _perfilColaboradorRepositorio.ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>())
            .Returns(perfilAsesor);

        _usuarioRepositorio.ObtenerPorIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns((Usuario)null!);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(solicitud.Id);
        resultado.AsesorId.Should().Be(asesorId);
        resultado.NombreAsesor.Should().BeNull();

        await _perfilColaboradorRepositorio.Received(1).ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>());
        await _usuarioRepositorio.Received(1).ObtenerPorIdAsync(usuarioId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteConAsesorYPerfilYUsuarioExisten_DebeRetornarDtoConNombreAsesor()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId,
            NivelAcademico.Pregrado,
            TipoTrabajo.Tesis,
            "Derecho",
            DateTime.UtcNow.AddDays(5),
            10,
            NormaCitacion.APA,
            IdiomaRequerido.Espanol,
            "PDF",
            "Instrucciones",
            false,
            false
        );
        var asesorId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorId);

        var usuarioId = Guid.NewGuid();
        var perfilAsesor = PerfilColaborador.Crear(usuarioId);
        
        var usuarioAsesor = Usuario.Crear(
            "asesor@grupoxpert.com",
            "hash_seguro",
            "Juan Asesor",
            "token_activacion",
            null,
            TipoUsuario.Asesor
        );

        var query = new ObtenerSolicitudPorIdQuery(solicitud.Id);

        _repositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        _perfilColaboradorRepositorio.ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>())
            .Returns(perfilAsesor);

        _usuarioRepositorio.ObtenerPorIdAsync(usuarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAsesor);

        // Act
        var resultado = await _manejador.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(solicitud.Id);
        resultado.AsesorId.Should().Be(asesorId);
        resultado.NombreAsesor.Should().Be("Juan Asesor");

        await _perfilColaboradorRepositorio.Received(1).ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>());
        await _usuarioRepositorio.Received(1).ObtenerPorIdAsync(usuarioId, Arg.Any<CancellationToken>());
    }
}
