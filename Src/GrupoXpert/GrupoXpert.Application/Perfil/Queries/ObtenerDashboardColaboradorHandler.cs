using GrupoXpert.Application.Perfil.Dtos;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Perfil.Queries;

public class ObtenerDashboardColaboradorHandler(
    IPerfilColaboradorRepository perfilColaboradorRepository,
    IDashboardColaboradorRepository dashboardRepository)
    : IRequestHandler<ObtenerDashboardColaboradorQuery, DashboardColaboradorDto?>
{
    public async Task<DashboardColaboradorDto?> Handle(
        ObtenerDashboardColaboradorQuery request,
        CancellationToken cancellationToken)
    {
        var perfil = await perfilColaboradorRepository
            .ObtenerPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

        if (perfil is null) return null;

        var resumen = await dashboardRepository
            .ObtenerResumenAsync(perfil.Id, cancellationToken);

        if (resumen is null) return null;

        var solicitudes = await dashboardRepository
            .ObtenerSolicitudesPorAsesorAsync(perfil.Id, cancellationToken);

        return new DashboardColaboradorDto
        {
            PerfilColaboradorId = perfil.Id,
            UsuarioId = perfil.UsuarioId,
            ProyectosEnCurso = resumen.ProyectosEnCurso,
            EntregasRealizadas = resumen.EntregasRealizadas,
            SolicitudesAbiertas = resumen.SolicitudesAbiertas,
            PostulacionesPendientes = resumen.PostulacionesPendientes,
            Reputacion = new ReputacionAcademicaDto
            {
                PuntajePromedio = resumen.Reputacion.PuntajePromedio,
                TotalCalificaciones = resumen.Reputacion.TotalCalificaciones,
                Nivel = resumen.Reputacion.Nivel.ToString()
            },
            Solicitudes = solicitudes.Select(s => new DetalleSolicitudAsesorDto
            {
                SolicitudId = s.SolicitudId,
                TipoTrabajo = s.TipoTrabajo,
                AreaTematica = s.AreaTematica,
                Estado = s.Estado,
                FechaEntrega = s.FechaEntrega,
                EsUrgente = s.EsUrgente,
                NumeroEntregas = s.NumeroEntregas,
                PuntajeCalificacion = s.PuntajeCalificacion
            }).ToList()
        };
    }
}