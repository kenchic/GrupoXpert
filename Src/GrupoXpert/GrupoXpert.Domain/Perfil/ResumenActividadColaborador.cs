using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Perfil;

public class ResumenActividadColaborador : ValueObject
{
    public int ProyectosEnCurso { get; private set; }
    public int EntregasRealizadas { get; private set; }
    public int SolicitudesAbiertas { get; private set; }
    public int PostulacionesPendientes { get; private set; }
    public ReputacionAcademica Reputacion { get; private set; }

    private ResumenActividadColaborador() { Reputacion = default!; }

    public ResumenActividadColaborador(
        int proyectosEnCurso,
        int entregasRealizadas,
        int solicitudesAbiertas,
        int postulacionesPendientes,
        ReputacionAcademica reputacion)
    {
        if (proyectosEnCurso < 0)
            throw new Exceptions.ExcepcionDominio("El número de proyectos en curso no puede ser negativo.");

        if (entregasRealizadas < 0)
            throw new Exceptions.ExcepcionDominio("El número de entregas realizadas no puede ser negativo.");

        if (solicitudesAbiertas < 0)
            throw new Exceptions.ExcepcionDominio("El número de solicitudes abiertas no puede ser negativo.");

        if (postulacionesPendientes < 0)
            throw new Exceptions.ExcepcionDominio("El número de postulaciones pendientes no puede ser negativo.");

        ProyectosEnCurso = proyectosEnCurso;
        EntregasRealizadas = entregasRealizadas;
        SolicitudesAbiertas = solicitudesAbiertas;
        PostulacionesPendientes = postulacionesPendientes;
        Reputacion = reputacion ?? ReputacionAcademica.SinCalificaciones;
    }

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return ProyectosEnCurso;
        yield return EntregasRealizadas;
        yield return SolicitudesAbiertas;
        yield return PostulacionesPendientes;
        yield return Reputacion;
    }
}