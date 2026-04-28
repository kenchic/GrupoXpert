using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando un usuario inicia sesión exitosamente.
/// </summary>
public sealed class SesionIniciadaEvento : IEventoDominio
{
    public Guid UsuarioId { get; }
    public string NombreUsuario { get; }
    public DateTime FechaOcurrencia { get; }

    public SesionIniciadaEvento(Guid usuarioId, string nombreUsuario)
    {
        UsuarioId = usuarioId;
        NombreUsuario = nombreUsuario;
        FechaOcurrencia = DateTime.UtcNow;
    }
}

