using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Calidad.Events;

public record RevisionRechazadaEvent(Guid RevisionId, Guid AvanceId, Guid RevisorId, string Observaciones) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
