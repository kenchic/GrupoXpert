using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Academia.Events;

public record SolicitudAcademicaCreadaDomainEvent(Guid SolicitudAcademicaId) : IDomainEvent
{
    public DateTime FechaOcurrencia { get; } = DateTime.UtcNow;
}
