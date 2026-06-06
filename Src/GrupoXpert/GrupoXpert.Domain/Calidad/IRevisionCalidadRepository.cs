namespace GrupoXpert.Domain.Calidad;

public interface IRevisionCalidadRepository
{
    Task<RevisionCalidad?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RevisionCalidad?> ObtenerPorAvanceAsync(Guid avanceId, CancellationToken cancellationToken = default);
    Task AgregarAsync(RevisionCalidad revision, CancellationToken cancellationToken = default);
    Task ActualizarAsync(RevisionCalidad revision, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevisionCalidad>> ObtenerPorRevisorAsync(Guid revisorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevisionCalidad>> ObtenerPendientesAsync(CancellationToken cancellationToken = default);
}
