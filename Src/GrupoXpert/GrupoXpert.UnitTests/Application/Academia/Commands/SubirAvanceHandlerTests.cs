using FluentAssertions;
using GrupoXpert.Application.Academia.Commands;
using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Identidad;
using NSubstitute;
using Xunit;

namespace GrupoXpert.UnitTests.Application.Academia.Commands;

public class SubirAvanceHandlerTests
{
    private readonly IAvanceRepository _avanceRepositorio;
    private readonly ISolicitudAcademicaRepository _solicitudRepositorio;
    private readonly GrupoXpert.Domain.Perfil.IPerfilColaboradorRepository _perfilColaboradorRepositorio;
    private readonly IUsuarioRepository _usuarioRepositorio;
    private readonly IArchivoStorageService _archivoStorage;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly SubirAvanceHandler _manejador;

    public SubirAvanceHandlerTests()
    {
        _avanceRepositorio = Substitute.For<IAvanceRepository>();
        _solicitudRepositorio = Substitute.For<ISolicitudAcademicaRepository>();
        _perfilColaboradorRepositorio = Substitute.For<GrupoXpert.Domain.Perfil.IPerfilColaboradorRepository>();
        _usuarioRepositorio = Substitute.For<IUsuarioRepository>();
        _archivoStorage = Substitute.For<IArchivoStorageService>();
        _unidadDeTrabajo = Substitute.For<IUnidadDeTrabajo>();
        _manejador = new SubirAvanceHandler(
            _avanceRepositorio,
            _solicitudRepositorio,
            _perfilColaboradorRepositorio,
            _usuarioRepositorio,
            _archivoStorage,
            _unidadDeTrabajo);
    }

    [Fact]
    public async Task Handle_CuandoSolicitudExisteYEstaEnProceso_DebeSubirAvanceYGuardarCambios()
    {
        var clienteId = Guid.NewGuid();
        var solicitud = SolicitudAcademica.Crear(
            clienteId, NivelAcademico.Pregrado, TipoTrabajo.Tesis, "Derecho Penal",
            DateTime.UtcNow.AddDays(15), 50, NormaCitacion.APA, IdiomaRequerido.Espanol,
            "PDF", "Instrucciones", false, false);
        var asesorId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorId);
        solicitud.IniciarTrabajo();

        var usuarioAsesor = Usuario.Crear(
            "asesor@test.com", "hash123", "Juan Pérez", "token", tipo: TipoUsuario.Asesor);
        var perfil = GrupoXpert.Domain.Perfil.PerfilColaborador.Crear(usuarioAsesor.Id);

        var comando = new SubirAvanceCommand(solicitud.Id, asesorId, "Avance del capítulo 1", 1, (int)TipoAvance.Parcial, Array.Empty<ArchivoAdjuntoInput>());

        _solicitudRepositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);
        _perfilColaboradorRepositorio.ObtenerPorIdAsync(asesorId, Arg.Any<CancellationToken>())
            .Returns(perfil);
        _usuarioRepositorio.ObtenerPorIdAsync(perfil.UsuarioId, Arg.Any<CancellationToken>())
            .Returns(usuarioAsesor);

        var resultado = await _manejador.Handle(comando, CancellationToken.None);

        resultado.Should().NotBeNull();
        resultado.Descripcion.Should().Be("Avance del capítulo 1");
        resultado.NumeroFase.Should().Be(1);
        resultado.Tipo.Should().Be((int)TipoAvance.Parcial);
        resultado.Estado.Should().Be((int)EstadoAvance.PendienteRevision);
        resultado.NombreAsesor.Should().Be("Juan Pérez");

        await _avanceRepositorio.Received(1).AgregarAsync(Arg.Any<Avance>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudNoExiste_DebeLanzarExcepcion()
    {
        var solicitudId = Guid.NewGuid();
        var asesorId = Guid.NewGuid();
        var comando = new SubirAvanceCommand(solicitudId, asesorId, "Avance", 1, (int)TipoAvance.Parcial, Array.Empty<ArchivoAdjuntoInput>());

        _solicitudRepositorio.ObtenerPorIdAsync(solicitudId, Arg.Any<CancellationToken>())
            .Returns((SolicitudAcademica?)null);

        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*solicitud academica no existe*");

        await _avanceRepositorio.DidNotReceive().AgregarAsync(Arg.Any<Avance>(), Arg.Any<CancellationToken>());
        await _unidadDeTrabajo.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoSolicitudEstaCancelada_DebeLanzarExcepcion()
    {
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "Derecho Penal",
            DateTime.UtcNow.AddDays(15), 50, NormaCitacion.APA, IdiomaRequerido.Espanol,
            "PDF", "Instrucciones", false, false);
        var asesorId = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorId);
        solicitud.Cancelar();

        var comando = new SubirAvanceCommand(solicitud.Id, asesorId, "Avance", 1, (int)TipoAvance.Parcial, Array.Empty<ArchivoAdjuntoInput>());

        _solicitudRepositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*en proceso*");

        await _avanceRepositorio.DidNotReceive().AgregarAsync(Arg.Any<Avance>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CuandoAsesorNoEsElAsignado_DebeLanzarExcepcion()
    {
        var solicitud = SolicitudAcademica.Crear(
            Guid.NewGuid(), NivelAcademico.Pregrado, TipoTrabajo.Tesis, "Derecho Penal",
            DateTime.UtcNow.AddDays(15), 50, NormaCitacion.APA, IdiomaRequerido.Espanol,
            "PDF", "Instrucciones", false, false);
        var asesorAsignado = Guid.NewGuid();
        var otroAsesor = Guid.NewGuid();
        solicitud.AsignarAsesor(asesorAsignado);
        solicitud.IniciarTrabajo();

        var comando = new SubirAvanceCommand(solicitud.Id, otroAsesor, "Avance no autorizado", 1, (int)TipoAvance.Parcial, Array.Empty<ArchivoAdjuntoInput>());

        _solicitudRepositorio.ObtenerPorIdAsync(solicitud.Id, Arg.Any<CancellationToken>())
            .Returns(solicitud);

        var accion = () => _manejador.Handle(comando, CancellationToken.None);

        await accion.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*asesor asignado*");

        await _avanceRepositorio.DidNotReceive().AgregarAsync(Arg.Any<Avance>(), Arg.Any<CancellationToken>());
    }
}
