namespace GrupoXpert.Domain.Common;

/// <summary>
/// Clase base para las Raíces de Agregado.
/// Extiende Entity con soporte para eventos de dominio.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _eventosDominio = [];

    /// <summary>
    /// Eventos de dominio pendientes de publicar.
    /// </summary>
    public IReadOnlyList<IDomainEvent> EventosDominio => _eventosDominio.AsReadOnly();

    protected AggregateRoot() : base() { }

    protected AggregateRoot(Guid id) : base(id) { }

    /// <summary>
    /// Registra un nuevo evento de dominio.
    /// </summary>
    protected void AgregarEventoDominio(IDomainEvent evento)
    {
        _eventosDominio.Add(evento);
    }

    /// <summary>
    /// Limpia todos los eventos de dominio pendientes (tras su publicación).
    /// </summary>
    public void LimpiarEventosDominio()
    {
        _eventosDominio.Clear();
    }
}
