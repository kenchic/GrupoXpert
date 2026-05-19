using GrupoXpert.Domain.Common;
using System;

namespace GrupoXpert.Domain.Perfil.Events;

public class PerfilColaboradorValidadoEvent : IDomainEvent
{
    public Guid PerfilColaboradorId { get; }
    public DateTime FechaOcurrencia { get; }

    public PerfilColaboradorValidadoEvent(Guid perfilColaboradorId)
    {
        PerfilColaboradorId = perfilColaboradorId;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
