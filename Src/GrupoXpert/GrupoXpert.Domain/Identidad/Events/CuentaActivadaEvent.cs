using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando un usuario activa su cuenta.
/// </summary>
public sealed class CuentaActivadaEvent : IDomainEvent
{
    public Guid UsuarioId { get; }
    public string Email { get; }
    public DateTime FechaOcurrencia { get; }

    public CuentaActivadaEvent(Guid usuarioId, string email)
    {
        UsuarioId = usuarioId;
        Email = email;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
