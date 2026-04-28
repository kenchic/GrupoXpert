namespace GrupoXpert.Domain.Common;

/// <summary>
/// Clase base para las Raíces de Agregado.
/// Extiende Entidad con soporte para eventos de dominio.
/// </summary>
public abstract class RaizAgregado : Entidad
{
    private readonly List<IEventoDominio> _eventosDominio = [];

    /// <summary>
    /// Eventos de dominio pendientes de publicar.
    /// </summary>
    public IReadOnlyList<IEventoDominio> EventosDominio => _eventosDominio.AsReadOnly();

    protected RaizAgregado() : base() { }

    protected RaizAgregado(Guid id) : base(id) { }

    /// <summary>
    /// Registra un nuevo evento de dominio.
    /// </summary>
    protected void AgregarEventoDominio(IEventoDominio evento)
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
