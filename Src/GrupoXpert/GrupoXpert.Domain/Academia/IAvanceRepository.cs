namespace GrupoXpert.Domain.Academia;

public interface IAvanceRepository
{
    Task<Avance?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AgregarAsync(Avance avance, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Avance avance, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Avance>> ObtenerPorSolicitudAsync(Guid solicitudId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Avance>> ObtenerPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<int> ContarEntregasPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
}
