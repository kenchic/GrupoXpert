using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando un usuario inicia sesión exitosamente.
/// </summary>
public sealed class SesionIniciadaEvent : IDomainEvent
{
    public Guid UsuarioId { get; }
    public string Email { get; }
    public DateTime FechaOcurrencia { get; }

    public SesionIniciadaEvent(Guid usuarioId, string email)
    {
        UsuarioId = usuarioId;
        Email = email;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
