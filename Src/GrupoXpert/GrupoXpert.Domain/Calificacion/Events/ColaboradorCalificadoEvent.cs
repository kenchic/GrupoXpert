using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Calificacion.Events;

public record ColaboradorCalificadoEvent(
    Guid CalificacionId,
    Guid SolicitudId,
    Guid ClienteId,
    Guid ColaboradorId,
    int Puntaje) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}