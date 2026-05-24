using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia.Events;

public record ComentarioAgregadoDomainEvent(Guid AvanceId, Guid ComentarioId, Guid AutorId) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
