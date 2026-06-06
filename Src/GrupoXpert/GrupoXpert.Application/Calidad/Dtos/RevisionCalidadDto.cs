namespace GrupoXpert.Application.Calidad.Dtos;

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
