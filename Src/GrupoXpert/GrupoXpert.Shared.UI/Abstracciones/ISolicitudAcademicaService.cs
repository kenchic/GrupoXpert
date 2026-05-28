using GrupoXpert.Shared.UI.Modelos.Academia;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface ISolicitudAcademicaService
{
    Task<bool> CrearSolicitudAsync(SolicitudAcademicaModelo modelo);
    Task<IReadOnlyList<SolicitudAcademicaDto>> ObtenerSolicitudesDashboardAsync();
    Task<SolicitudAcademicaDto?> ObtenerPorIdAsync(Guid id);
    Task<bool> AsignarAsesorAsync(Guid solicitudId, Guid asesorId);
    Task<bool> PostularASolicitudAsync(Guid solicitudId, Guid colaboradorId);
    Task<bool> SeleccionarPostuladoAsync(Guid solicitudId, Guid colaboradorId);

    Task<IReadOnlyList<AvanceDto>> ObtenerAvancesAsync(Guid solicitudId);
    Task<AvanceDto?> SubirAvanceAsync(Guid solicitudId, Guid asesorId, string descripcion, int numeroFase, int tipo, IReadOnlyList<ArchivoSubidaModelo> archivos);
    Task<bool> AgregarComentarioAsync(Guid avanceId, Guid autorId, string contenido);
    Task<bool> AgregarArchivoAsync(Guid avanceId, string nombreArchivo, string url, long tamanioBytes, string tipoContenido);
    Task<bool> AprobarAvanceAsync(Guid avanceId);
    Task<bool> RechazarAvanceAsync(Guid avanceId);
}
