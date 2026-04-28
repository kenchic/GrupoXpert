using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando se crea un nuevo usuario en el sistema.
/// </summary>
public sealed class UsuarioCreadoEvento : IEventoDominio
{
    public Guid UsuarioId { get; }
    public string NombreUsuario { get; }
    public DateTime FechaOcurrencia { get; }

    public UsuarioCreadoEvento(Guid usuarioId, string nombreUsuario)
    {
        UsuarioId = usuarioId;
        NombreUsuario = nombreUsuario;
        FechaOcurrencia = DateTime.UtcNow;
    }
}

