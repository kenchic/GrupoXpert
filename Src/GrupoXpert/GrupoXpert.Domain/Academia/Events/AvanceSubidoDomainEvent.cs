using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia.Events;

public record AvanceSubidoDomainEvent(Guid AvanceId, Guid SolicitudId, Guid AsesorId) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
