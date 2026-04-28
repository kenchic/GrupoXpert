namespace GrupoXpert.Domain.Exceptions;

/// <summary>
/// Excepción base para todas las violaciones de reglas del dominio.
/// </summary>
public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje) { }

    public ExcepcionDominio(string mensaje, Exception excepcionInterna)
        : base(mensaje, excepcionInterna) { }
}
