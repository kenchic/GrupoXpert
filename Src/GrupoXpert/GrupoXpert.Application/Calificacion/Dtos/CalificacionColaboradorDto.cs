namespace GrupoXpert.Application.Calificacion.Dtos;

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