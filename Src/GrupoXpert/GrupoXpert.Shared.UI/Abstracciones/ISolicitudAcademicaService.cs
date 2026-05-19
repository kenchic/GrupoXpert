using GrupoXpert.Shared.UI.Modelos.Academia;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface ISolicitudAcademicaService
{
    Task<bool> CrearSolicitudAsync(SolicitudAcademicaModelo modelo);
    Task<IReadOnlyList<SolicitudAcademicaDto>> ObtenerSolicitudesDashboardAsync();
}
