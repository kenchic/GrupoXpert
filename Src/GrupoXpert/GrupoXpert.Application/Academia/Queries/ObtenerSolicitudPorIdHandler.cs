using System;
using System.Threading;
using System.Threading.Tasks;
using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Perfil;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Academia.Queries;

public sealed class ObtenerSolicitudPorIdHandler(
    ISolicitudAcademicaRepository repositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerSolicitudPorIdQuery, SolicitudAcademicaDto?>
{
    public async Task<SolicitudAcademicaDto?> Handle(ObtenerSolicitudPorIdQuery request, CancellationToken cancellationToken)
    {
        var solicitud = await repositorio.ObtenerPorIdAsync(request.Id, cancellationToken);
        if (solicitud == null) return null;

        string? nombreAsesor = null;
        if (solicitud.AsesorId.HasValue)
        {
            var perfilAsesor = await perfilColaboradorRepositorio.ObtenerPorIdAsync(solicitud.AsesorId.Value, cancellationToken);
            if (perfilAsesor != null)
            {
                var usuarioAsesor = await usuarioRepositorio.ObtenerPorIdAsync(perfilAsesor.UsuarioId, cancellationToken);
                if (usuarioAsesor != null)
                {
                    nombreAsesor = usuarioAsesor.Nombre;
                }
            }
        }

        return new SolicitudAcademicaDto(
            solicitud.Id,
            solicitud.ClienteId,
            (int)solicitud.NivelAcademico,
            (int)solicitud.TipoTrabajo,
            solicitud.AreaTematica,
            solicitud.FechaEntrega,
            solicitud.NumeroPaginasOPalabras,
            (int)solicitud.NormaCitacion,
            (int)solicitud.Idioma,
            solicitud.FormatoRequerido,
            solicitud.MaterialBase,
            solicitud.EsUrgente,
            solicitud.EntregaPorFases,
            (int)solicitud.Estado,
            solicitud.AsesorId,
            nombreAsesor
        );
    }
}
