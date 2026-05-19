namespace GrupoXpert.Domain.Academia;

public interface ISolicitudAcademicaRepository
{
    Task<SolicitudAcademica?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AgregarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task ActualizarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPendientesYAsignadasAAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorEstadosAsync(IEnumerable<EstadoSolicitud> estados, CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken = default);
}
