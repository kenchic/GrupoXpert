namespace GrupoXpert.Domain.Common;

/// <summary>
/// Clase base abstracta para Objetos de Valor.
/// La igualdad se determina por la comparación de todos sus componentes.
/// </summary>
public abstract class ObjetoValor
{
    /// <summary>
    /// Obtiene los componentes que definen la igualdad del objeto de valor.
    /// </summary>
    protected abstract IEnumerable<object?> ObtenerComponentesIgualdad();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var otro = (ObjetoValor)obj;

        return ObtenerComponentesIgualdad()
            .SequenceEqual(otro.ObtenerComponentesIgualdad());
    }

    public override int GetHashCode()
    {
        return ObtenerComponentesIgualdad()
            .Aggregate(1, (actual, componente) =>
            {
                unchecked
                {
                    return actual * 23 + (componente?.GetHashCode() ?? 0);
                }
            });
    }

    public static bool operator ==(ObjetoValor? izquierda, ObjetoValor? derecha)
    {
        if (izquierda is null && derecha is null)
            return true;

        if (izquierda is null || derecha is null)
            return false;

        return izquierda.Equals(derecha);
    }

    public static bool operator !=(ObjetoValor? izquierda, ObjetoValor? derecha) => !(izquierda == derecha);
}
