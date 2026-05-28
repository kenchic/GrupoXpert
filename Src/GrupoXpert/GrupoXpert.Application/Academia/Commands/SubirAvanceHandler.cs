using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class SubirAvanceHandler(
    IAvanceRepository avanceRepositorio,
    ISolicitudAcademicaRepository solicitudRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio,
    IArchivoStorageService archivoStorage,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<SubirAvanceCommand, AvanceDto>
{
    public async Task<AvanceDto> Handle(SubirAvanceCommand comando, CancellationToken cancelacion)
    {
        var solicitud = await solicitudRepositorio.ObtenerPorIdAsync(comando.SolicitudId, cancelacion)
            ?? throw new InvalidOperationException("La solicitud academica no existe.");

        if (solicitud.Estado != EstadoSolicitud.EnProceso || solicitud.Estado != EstadoSolicitud.Asignada)
            throw new InvalidOperationException("Solo se pueden subir avances a solicitudes en proceso.");

        if (solicitud.AsesorId != comando.AsesorId)
            throw new InvalidOperationException("Solo el asesor asignado puede subir avances a esta solicitud.");

        var avance = Avance.Subir(
            comando.SolicitudId,
            comando.AsesorId,
            comando.Descripcion,
            comando.NumeroFase,
            (TipoAvance)comando.Tipo);

        foreach (var archivoInput in comando.Archivos)
        {
            var url = await archivoStorage.GuardarAsync(
                archivoInput.NombreArchivo,
                archivoInput.Contenido,
                archivoInput.TipoContenido,
                cancelacion);

            avance.AgregarArchivo(
                archivoInput.NombreArchivo,
                url,
                archivoInput.TamanioBytes,
                archivoInput.TipoContenido);
        }

        await avanceRepositorio.AgregarAsync(avance, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        var perfilAsesor = await perfilColaboradorRepositorio.ObtenerPorIdAsync(avance.AsesorId, cancelacion);
        var nombreAsesor = "Desconocido";
        if (perfilAsesor is not null)
        {
            var usuario = await usuarioRepositorio.ObtenerPorIdAsync(perfilAsesor.UsuarioId, cancelacion);
            if (usuario is not null)
                nombreAsesor = usuario.Nombre;
        }

        return new AvanceDto(
            avance.Id,
            avance.SolicitudId,
            avance.AsesorId,
            nombreAsesor,
            avance.Descripcion,
            avance.NumeroFase,
            (int)avance.Tipo,
            (int)avance.Estado,
            avance.FechaSubida,
            Array.Empty<ComentarioDto>(),
            avance.ArchivosAdjuntos.Select(a => new ArchivoAdjuntoDto(
                a.Id,
                a.AvanceId,
                a.NombreArchivo,
                a.Url,
                a.TamanioBytes,
                a.TipoContenido,
                a.FechaSubida)).ToList());
    }
}
