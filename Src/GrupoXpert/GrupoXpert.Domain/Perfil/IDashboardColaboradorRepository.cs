namespace GrupoXpert.Domain.Perfil;

public interface IDashboardColaboradorRepository
{
    Task<ResumenActividadColaborador?> ObtenerResumenAsync(Guid perfilColaboradorId, CancellationToken cancellationToken = default);
    Task<ReputacionAcademica> ObtenerReputacionAsync(Guid perfilColaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DetalleSolicitudAsesor>> ObtenerSolicitudesPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
}