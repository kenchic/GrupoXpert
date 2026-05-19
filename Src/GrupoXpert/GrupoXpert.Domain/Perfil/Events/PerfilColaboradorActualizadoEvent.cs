using GrupoXpert.Domain.Common;
using System;

namespace GrupoXpert.Domain.Perfil.Events;

public class PerfilColaboradorActualizadoEvent : IDomainEvent
{
    public Guid PerfilColaboradorId { get; }
    public DateTime FechaOcurrencia { get; }

    public PerfilColaboradorActualizadoEvent(Guid perfilColaboradorId)
    {
        PerfilColaboradorId = perfilColaboradorId;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
