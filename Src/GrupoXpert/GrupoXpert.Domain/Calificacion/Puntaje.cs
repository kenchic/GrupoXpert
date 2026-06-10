using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Calificacion;

public class Puntaje : ValueObject
{
    public int Valor { get; private set; }

    private Puntaje() { }

    public Puntaje(int valor)
    {
        if (valor < 1 || valor > 5)
            throw new ExcepcionDominio("El puntaje debe estar entre 1 y 5 estrellas.");

        Valor = valor;
    }

    public static Puntaje UnaEstrella => new(1);
    public static Puntaje DosEstrellas => new(2);
    public static Puntaje TresEstrellas => new(3);
    public static Puntaje CuatroEstrellas => new(4);
    public static Puntaje CincoEstrellas => new(5);

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return Valor;
    }
}