namespace GrupoXpert.Application.Calificacion.Dtos;

public sealed record SolicitudPorCalificarDto(
    Guid SolicitudId,
    Guid ClienteId,
    string AreaTematica,
    int Estado,
    Guid? AsesorId,
    string? NombreAsesor,
    bool Calificada,
    int? Puntaje,
    string? ObservacionCalificacion,
    DateTime? FechaCalificacion
);