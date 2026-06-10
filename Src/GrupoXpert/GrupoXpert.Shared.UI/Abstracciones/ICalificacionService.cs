using GrupoXpert.Shared.UI.Modelos.Calificacion;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface ICalificacionService
{
    Task<CalificacionColaboradorDto?> CalificarAsync(Guid solicitudId, Guid clienteId, Guid colaboradorId, int puntaje, string? observacion);
    Task<IReadOnlyList<SolicitudPorCalificarDto>> ObtenerSolicitudesPorCalificarAsync();
    Task<CalificacionColaboradorDto?> ObtenerPorSolicitudAsync(Guid solicitudId);
    Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerPorColaboradorAsync(Guid colaboradorId);
    Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerMisCalificacionesAsync();
    Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerTodasAsync();
    Task<bool> LiberarSolicitudAsync(Guid solicitudId);
}