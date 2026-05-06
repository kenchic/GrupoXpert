namespace GrupoXpert.Application.Common.Interfaces;

/// <summary>
/// Interfaz para el patrón Unidad de Trabajo.
/// Asegura que todas las operaciones de persistencia en una transacción se completen con éxito.
/// </summary>
public interface IUnidadDeTrabajo
{
    /// <summary>
    /// Guarda todos los cambios realizados en el contexto de persistencia.
    /// </summary>
    Task<int> GuardarCambiosAsync(CancellationToken cancelacion = default);
}

