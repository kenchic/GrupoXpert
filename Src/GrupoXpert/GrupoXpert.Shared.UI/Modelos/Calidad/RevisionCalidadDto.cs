namespace GrupoXpert.Shared.UI.Modelos.Calidad;

public sealed record RevisionCalidadDto(
    Guid Id,
    Guid AvanceId,
    Guid RevisorId,
    string? NombreRevisor,
    DateTime FechaRevision,
    bool VistoBueno,
    string? Observaciones,
    int Estado
);
