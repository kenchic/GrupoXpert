namespace GrupoXpert.Shared.UI.Modelos.Calificacion;

public sealed record CalificacionColaboradorDto(
    Guid Id,
    Guid SolicitudId,
    Guid ClienteId,
    Guid ColaboradorId,
    string? NombreColaborador,
    int Puntaje,
    string? Observacion,
    DateTime FechaCalificacion
);

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