using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando se crea un nuevo usuario en el sistema.
/// Incluye el email para que los handlers de infraestructura envíen el enlace de activación.
/// </summary>
public sealed class UsuarioCreadoEvent : IDomainEvent
{
    public Guid UsuarioId { get; }
    public string Email { get; }
    public string TokenActivacion { get; }
    public DateTime FechaOcurrencia { get; }

    public UsuarioCreadoEvent(Guid usuarioId, string email, string tokenActivacion)
    {
        UsuarioId = usuarioId;
        Email = email;
        TokenActivacion = tokenActivacion;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
