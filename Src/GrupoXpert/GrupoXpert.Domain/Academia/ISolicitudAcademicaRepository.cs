namespace GrupoXpert.Domain.Academia;

public interface ISolicitudAcademicaRepository
{
    Task<SolicitudAcademica?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AgregarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task ActualizarAsync(SolicitudAcademica solicitud, CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken = default);
}
