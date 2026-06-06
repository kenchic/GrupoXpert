using GrupoXpert.Shared.UI.Modelos.Calidad;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IRevisionCalidadService
{
    Task<RevisionCalidadDto?> CrearRevisionAsync(Guid avanceId, Guid revisorId);
    Task<RevisionCalidadDto?> OtorgarVistoBuenoAsync(Guid revisionId);
    Task<RevisionCalidadDto?> RechazarRevisionAsync(Guid revisionId, string observaciones);
    Task<IReadOnlyList<RevisionCalidadDto>> ObtenerPendientesAsync();
    Task<RevisionCalidadDto?> ObtenerPorAvanceAsync(Guid avanceId);
}
