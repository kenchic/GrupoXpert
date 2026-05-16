using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Perfil.Events;

public class PerfilClienteActualizadoEvent : IDomainEvent
{
    public Guid PerfilClienteId { get; }

    public DateTime FechaOcurrencia => throw new NotImplementedException();

    public PerfilClienteActualizadoEvent(Guid perfilClienteId)
    {
        PerfilClienteId = perfilClienteId;
    }
}
