using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Academia.Queries;

/// <summary>
/// Consulta para obtener las solicitudes académicas correspondientes al dashboard de un usuario según su rol.
/// </summary>
public record ObtenerSolicitudesDashboardQuery(Guid UsuarioId, TipoUsuario Rol) 
    : IRequest<IReadOnlyList<SolicitudAcademicaDto>>;

public sealed class ObtenerSolicitudesDashboardHandler(
    ISolicitudAcademicaRepository solicitudRepository,
    IPerfilClienteRepository perfilClienteRepository,
    IPerfilColaboradorRepository perfilColaboradorRepository)
    : IRequestHandler<ObtenerSolicitudesDashboardQuery, IReadOnlyList<SolicitudAcademicaDto>>
{
    public async Task<IReadOnlyList<SolicitudAcademicaDto>> Handle(
        ObtenerSolicitudesDashboardQuery request, 
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SolicitudAcademica> solicitudes = Array.Empty<SolicitudAcademica>();

        if (request.Rol == TipoUsuario.Estudiante)
        {
            var perfil = await perfilClienteRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
            if (perfil != null)
            {
                solicitudes = await solicitudRepository.ObtenerPorClienteAsync(perfil.Id, cancellationToken);
            }
        }
        else if (request.Rol == TipoUsuario.Asesor)
        {
            var perfil = await perfilColaboradorRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);
            if (perfil != null)
            {
                solicitudes = await solicitudRepository.ObtenerPendientesYAsignadasAAsesorAsync(perfil.Id, cancellationToken);
            }
        }
        else if (request.Rol == TipoUsuario.Administrador)
        {
            solicitudes = await solicitudRepository.ObtenerPorEstadosAsync(
                new[] { EstadoSolicitud.Pendiente, EstadoSolicitud.EnProceso, EstadoSolicitud.Asignada }, 
                cancellationToken);
        }
        else if (request.Rol == TipoUsuario.Revisor)
        {
            solicitudes = await solicitudRepository.ObtenerConAvanceFinalPendienteRevisionAsync(cancellationToken);
        }

        return solicitudes.Select(x => new SolicitudAcademicaDto(
            x.Id,
            x.ClienteId,
            (int)x.NivelAcademico,
            (int)x.TipoTrabajo,
            x.AreaTematica,
            x.FechaEntrega,
            x.NumeroPaginasOPalabras,
            (int)x.NormaCitacion,
            (int)x.Idioma,
            x.FormatoRequerido,
            x.MaterialBase,
            x.EsUrgente,
            x.EntregaPorFases,
            (int)x.Estado,
            x.AsesorId
        )).ToList().AsReadOnly();
    }
}
