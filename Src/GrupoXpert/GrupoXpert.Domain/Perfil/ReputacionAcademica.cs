using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Perfil;

public class ReputacionAcademica : ValueObject
{
    public decimal PuntajePromedio { get; private set; }
    public int TotalCalificaciones { get; private set; }
    public NivelReputacion Nivel { get; private set; }

    private ReputacionAcademica() { }

    public ReputacionAcademica(decimal puntajePromedio, int totalCalificaciones)
    {
        if (puntajePromedio < 0 || puntajePromedio > 5)
            throw new ExcepcionDominio("El puntaje promedio de reputación debe estar entre 0 y 5.");

        if (totalCalificaciones < 0)
            throw new ExcepcionDominio("El total de calificaciones no puede ser negativo.");

        PuntajePromedio = puntajePromedio;
        TotalCalificaciones = totalCalificaciones;
        Nivel = DeterminarNivel(puntajePromedio, totalCalificaciones);
    }

    public static ReputacionAcademica SinCalificaciones => new(0m, 0);

    public ReputacionAcademica Recalcular(IEnumerable<int> puntajes)
    {
        var lista = puntajes.ToList();

        if (lista.Count == 0)
            return SinCalificaciones;

        var promedio = Math.Round((decimal)lista.Average(), 2);
        return new ReputacionAcademica(promedio, lista.Count);
    }

    private static NivelReputacion DeterminarNivel(decimal puntaje, int totalCalificaciones)
    {
        if (totalCalificaciones == 0)
            return NivelReputacion.SinCalificar;

        if (puntaje >= 4.5m) return NivelReputacion.Experto;
        if (puntaje >= 3.5m) return NivelReputacion.Avanzado;
        if (puntaje >= 2.5m) return NivelReputacion.Intermedio;
        return NivelReputacion.Principiante;
    }

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return PuntajePromedio;
        yield return TotalCalificaciones;
        yield return Nivel;
    }
}