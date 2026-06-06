using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Calidad.Events;

public record RevisionCalidadCreadaEvent(Guid RevisionId, Guid AvanceId, Guid RevisorId) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
