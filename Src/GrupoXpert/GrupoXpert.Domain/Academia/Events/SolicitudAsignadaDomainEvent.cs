using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia.Events;

public record SolicitudAsignadaDomainEvent(Guid SolicitudAcademicaId, Guid AsesorId) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
