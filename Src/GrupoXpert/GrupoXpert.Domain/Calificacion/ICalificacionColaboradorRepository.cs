namespace GrupoXpert.Domain.Calificacion;

public interface ICalificacionColaboradorRepository
{
    Task<CalificacionColaborador?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CalificacionColaborador?> ObtenerPorSolicitudAsync(Guid solicitudId, CancellationToken cancellationToken = default);
    Task AgregarAsync(CalificacionColaborador calificacion, CancellationToken cancellationToken = default);
    Task ActualizarAsync(CalificacionColaborador calificacion, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CalificacionColaborador>> ObtenerPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CalificacionColaborador>> ObtenerPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CalificacionColaborador>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<decimal> ObtenerPuntajePromedioPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<int> ContarCalificacionesPorColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
}