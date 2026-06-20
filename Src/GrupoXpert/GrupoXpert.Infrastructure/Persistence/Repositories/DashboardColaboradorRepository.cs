using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Perfil;
using Microsoft.EntityFrameworkCore;

namespace GrupoXpert.Infrastructure.Persistence.Repositories;

public sealed class DashboardColaboradorRepository(
    AppDbContext context,
    ISolicitudAcademicaRepository solicitudRepo,
    IAvanceRepository avanceRepo,
    ICalificacionColaboradorRepository calificacionRepo) : IDashboardColaboradorRepository
{
    public async Task<ResumenActividadColaborador?> ObtenerResumenAsync(Guid perfilColaboradorId, CancellationToken cancellationToken = default)
    {
        var perfil = await context.Set<PerfilColaborador>()
            .FirstOrDefaultAsync(p => p.Id == perfilColaboradorId, cancellationToken);

        if (perfil is null)
            return null;

        var colaboradorId = perfil.Id;

        var proyectosEnCurso = await solicitudRepo.ContarEnProcesoPorAsesorAsync(colaboradorId, cancellationToken);
        var entregasRealizadas = await avanceRepo.ContarEntregasPorAsesorAsync(colaboradorId, cancellationToken);
        var solicitudesAbiertas = await solicitudRepo.ContarAbiertasPorAsesorAsync(colaboradorId, cancellationToken);
        var postulacionesPendientes = await solicitudRepo.ContarPostulacionesPendientesPorColaboradorAsync(colaboradorId, cancellationToken);
        var reputacion = await ObtenerReputacionAsync(colaboradorId, cancellationToken);

        return new ResumenActividadColaborador(
            proyectosEnCurso,
            entregasRealizadas,
            solicitudesAbiertas,
            postulacionesPendientes,
            reputacion);
    }

    public async Task<ReputacionAcademica> ObtenerReputacionAsync(Guid perfilColaboradorId, CancellationToken cancellationToken = default)
    {
        var puntajePromedio = await calificacionRepo.ObtenerPuntajePromedioPorColaboradorAsync(perfilColaboradorId, cancellationToken);
        var totalCalificaciones = await calificacionRepo.ContarCalificacionesPorColaboradorAsync(perfilColaboradorId, cancellationToken);

        return new ReputacionAcademica(puntajePromedio, totalCalificaciones);
    }

    public async Task<IReadOnlyList<DetalleSolicitudAsesor>> ObtenerSolicitudesPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default)
    {
        var solicitudes = await solicitudRepo.ObtenerAsignadasAAsesorAsync(asesorId, cancellationToken);

        var detalles = new List<DetalleSolicitudAsesor>();

        foreach (var solicitud in solicitudes)
        {
            var avances = await avanceRepo.ObtenerPorSolicitudAsync(solicitud.Id, cancellationToken);
            var numeroEntregas = avances.Count;

            var calificacion = await calificacionRepo.ObtenerPorSolicitudAsync(solicitud.Id, cancellationToken);

            detalles.Add(new DetalleSolicitudAsesor(
                solicitud.Id,
                solicitud.TipoTrabajo.ToString(),
                solicitud.AreaTematica,
                solicitud.Estado.ToString(),
                solicitud.FechaEntrega,
                solicitud.EsUrgente,
                numeroEntregas,
                calificacion?.Puntaje.Valor));
        }

        return detalles;
    }
}