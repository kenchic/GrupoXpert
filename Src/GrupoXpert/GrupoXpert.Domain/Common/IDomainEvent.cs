namespace GrupoXpert.Domain.Common;

/// <summary>
/// Marcador para eventos de dominio.
/// Todos los eventos del dominio deben implementar esta interfaz.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Fecha y hora (UTC) en que ocurrió el evento.
    /// </summary>
    DateTime FechaOcurrencia { get; }
}
