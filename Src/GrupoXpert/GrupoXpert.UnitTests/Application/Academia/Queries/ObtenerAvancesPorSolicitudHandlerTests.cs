using FluentAssertions;
using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Application.Academia.Queries;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Queries;

public class ObtenerAvancesPorSolicitudHandlerTests
{
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly GrupoXpert.Domain.Perfil.IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly ObtenerAvancesPorSolicitudHandler _manejador;

    public ObtenerAvancesPorSolicitudHandlerTests()
    {
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _perfilColaboradorRepositorio = Substitute.For<GrupoXpert.Domain.Perfil.IPerfilColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _manejador = new ObtenerAvancesPorSolicitudHandler(
            _avanceRepositorio, _perfilColaboradorRepositorio, _usuarioRepositorio);
    }

    [Fact]
    public async Task Handle_CuandoHayAvances_DebeRetornarListaConNombresResueltos()
    {
        var solicitudId = Guid.NewGuid();
        var avance = Avance.Subir(solicitudId, Guid.NewGuid(), "Avance 1", 1, TipoAvance.Parcial);

        var usuarioAsesor = Usuario.Crear(
            "asesor@test.com", "hash", "María López", "token", tipo: TipoUsuario.Asesor);
        var perfil = GrupoXpert.Domain.Perfil.PerfilColaborador.Crear(usuarioAsesor.Id);

        _avanceRepositorio.ObtenerPorSolicitudAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(new List<Avance> { avance });
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(avance.AsesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAsesor);

        var consulta = new ObtenerAvancesPorSolicitudQuery(solicitudId);
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        resultado.Should().HaveCount(1);
        resultado[0].Id.Should().Be(avance.Id);
        resultado[0].Descripcion.Should().Be("Avance 1");
        resultado[0].NombreAsesor.Should().Be("María López");
        resultado[0].Tipo.Should().Be((int)TipoAvance.Parcial);
        resultado[0].Estado.Should().Be((int)EstadoAvance.PendienteRevision);
    }

    [Fact]
    public async Task Handle_CuandoNoHayAvances_DebeRetornarListaVacia()
    {
        var solicitudId = Guid.NewGuid();

        _avanceRepositorio.ObtenerPorSolicitudAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(new List<Avance>());

        var consulta = new ObtenerAvancesPorSolicitudQuery(solicitudId);
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CuandoAvanceTieneComentarios_DebeIncluirComentariosConNombres()
    {
        var solicitudId = Guid.NewGuid();
        var avance = Avance.Subir(solicitudId, Guid.NewGuid(), "Avance con comentarios", 1, TipoAvance.Parcial);
        var autorComentarioId = Guid.NewGuid();
        avance.AgregarComentario(autorComentarioId, "Excelente trabajo");

        var usuarioAsesor = Usuario.Crear("a@test.com", "hash", "María", "tok", tipo: TipoUsuario.Asesor);
        var perfil = GrupoXpert.Domain.Perfil.PerfilColaborador.Crear(usuarioAsesor.Id);
        var usuarioAutor = Usuario.Crear("b@test.com", "hash", "Carlos", "tok", tipo: TipoUsuario.Estudiante);

        _avanceRepositorio.ObtenerPorSolicitudAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(new List<Avance> { avance });
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(avance.AsesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAsesor);
        _usuarioRepositorio.ObtenerPorIdAsync(autorComentarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAutor);

        var consulta = new ObtenerAvancesPorSolicitudQuery(solicitudId);
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        resultado[0].Comentarios.Should().HaveCount(1);
        resultado[0].Comentarios[0].Contenido.Should().Be("Excelente trabajo");
        resultado[0].Comentarios[0].NombreAutor.Should().Be("Carlos");
    }

    [Fact]
    public async Task Handle_CuandoAvanceTieneArchivosAdjuntos_DebeIncluirArchivos()
    {
        var solicitudId = Guid.NewGuid();
        var avance = Avance.Subir(solicitudId, Guid.NewGuid(), "Avance con archivos", 1, TipoAvance.Final);
        avance.AgregarArchivo("tesis.pdf", "https://url.com/tesis.pdf", 2048000, "application/pdf");

        var usuarioAsesor = Usuario.Crear("a@test.com", "hash", "María", "tok", tipo: TipoUsuario.Asesor);
        var perfil = GrupoXpert.Domain.Perfil.PerfilColaborador.Crear(usuarioAsesor.Id);

        _avanceRepositorio.ObtenerPorSolicitudAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(new List<Avance> { avance });
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(avance.AsesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAsesor);

        var consulta = new ObtenerAvancesPorSolicitudQuery(solicitudId);
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        resultado[0].ArchivosAdjuntos.Should().HaveCount(1);
        resultado[0].ArchivosAdjuntos[0].NombreArchivo.Should().Be("tesis.pdf");
        resultado[0].ArchivosAdjuntos[0].TamanioBytes.Should().Be(2048000);
        resultado[0].ArchivosAdjuntos[0].TipoContenido.Should().Be("application/pdf");
        resultado[0].Tipo.Should().Be((int)TipoAvance.Final);
    }

    [Fact]
    public async Task Handle_MultiplesAvances_DebeRetornarOrdenados()
    {
        var solicitudId = Guid.NewGuid();
        var avance1 = Avance.Subir(solicitudId, Guid.NewGuid(), "Primer avance", 1, TipoAvance.Parcial);
        var avance2 = Avance.Subir(solicitudId, Guid.NewGuid(), "Segundo avance", 2, TipoAvance.Parcial);

        var usuario = Usuario.Crear("a@test.com", "hash", "Pedro", "tok", tipo: TipoUsuario.Asesor);
        var perfil = GrupoXpert.Domain.Perfil.PerfilColaborador.Crear(usuario.Id);

        _avanceRepositorio.ObtenerPorSolicitudAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns(new List<Avance> { avance1, avance2 });
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(avance1.AsesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(avance2.AsesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>())
            .Returns(usuario);

        var consulta = new ObtenerAvancesPorSolicitudQuery(solicitudId);
        var resultado = await _manejador.Handle(consulta, CancellationToken.None);

        resultado.Should().HaveCount(2);
        resultado[0].Descripcion.Should().Be("Primer avance");
        resultado[1].Descripcion.Should().Be("Segundo avance");
    }
}
