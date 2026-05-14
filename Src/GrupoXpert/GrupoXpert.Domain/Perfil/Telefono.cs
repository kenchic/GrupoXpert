using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Perfil;

public class Telefono : ValueObject
{
    public string Numero { get; }

    private Telefono(string numero)
    {
        Numero = numero;
    }

    public static Telefono Crear(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de teléfono no puede estar vacío.", nameof(numero));
            
        return new Telefono(numero);
    }

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return Numero;
    }
}
