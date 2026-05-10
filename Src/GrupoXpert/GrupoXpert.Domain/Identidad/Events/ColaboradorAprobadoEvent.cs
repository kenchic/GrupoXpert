using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Identidad.Events;

/// <summary>
/// Evento de dominio emitido cuando un administrador aprueba la cuenta de un colaborador (Asesor).
/// </summary>
public sealed class ColaboradorAprobadoEvent : IDomainEvent
{
    public Guid ColaboradorId { get; }
    public string Correo { get; }
    public Guid AdministradorId { get; }
    public DateTime FechaOcurrencia { get; }

    public ColaboradorAprobadoEvent(Guid colaboradorId, string correo, Guid administradorId)
    {
        ColaboradorId = colaboradorId;
        Correo = correo;
        AdministradorId = administradorId;
        FechaOcurrencia = DateTime.UtcNow;
    }
}
