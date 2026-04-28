namespace GrupoXpert.Domain.Common;

/// <summary>
/// Clase base abstracta para todas las Entidades del dominio.
/// Proporciona identidad (Id) e igualdad basada en identidad.
/// </summary>
public abstract class Entidad
{
    public Guid Id { get; protected set; }

    protected Entidad()
    {
        Id = Guid.NewGuid();
    }

    protected Entidad(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entidad otra)
            return false;

        if (ReferenceEquals(this, otra))
            return true;

        if (GetType() != otra.GetType())
            return false;

        return Id == otra.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entidad? izquierda, Entidad? derecha)
    {
        if (izquierda is null && derecha is null)
            return true;

        if (izquierda is null || derecha is null)
            return false;

        return izquierda.Equals(derecha);
    }

    public static bool operator !=(Entidad? izquierda, Entidad? derecha) => !(izquierda == derecha);
}
