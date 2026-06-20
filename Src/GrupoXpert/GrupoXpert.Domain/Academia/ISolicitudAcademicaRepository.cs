namespace GrupoXpert.Domain.Academia;

public interface ISolicitudAcademicaRepository
{
    Task<SolicitudAcademica?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AgregarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task ActualizarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPendientesYAsignadasAAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerPorEstadosAsync(IEnumerable<EstadoSolicitud> estados, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerConAvanceFinalPendienteRevisionAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerEnLiberacionPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerEnLiberacionAsync(CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> ContarEnProcesoPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<int> ContarAbiertasPorAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAcademica>> ObtenerAsignadasAAsesorAsync(Guid asesorId, CancellationToken cancellationToken = default);
    Task<int> ContarPostulacionesPendientesPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
}
