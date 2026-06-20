using GrupoXpert.Domain.Common;

namespace GrupoXpert.Domain.Perfil;

public class DetalleSolicitudAsesor : ValueObject
{
    public Guid SolicitudId { get; private set; }
    public string TipoTrabajo { get; private set; }
    public string AreaTematica { get; private set; }
    public string Estado { get; private set; }
    public DateTime FechaEntrega { get; private set; }
    public bool EsUrgente { get; private set; }
    public int NumeroEntregas { get; private set; }
    public int? PuntajeCalificacion { get; private set; }

    private DetalleSolicitudAsesor() { TipoTrabajo = string.Empty; AreaTematica = string.Empty; Estado = string.Empty; }

    public DetalleSolicitudAsesor(
        Guid solicitudId,
        string tipoTrabajo,
        string areaTematica,
        string estado,
        DateTime fechaEntrega,
        bool esUrgente,
        int numeroEntregas,
        int? puntajeCalificacion)
    {
        SolicitudId = solicitudId;
        TipoTrabajo = tipoTrabajo ?? string.Empty;
        AreaTematica = areaTematica ?? string.Empty;
        Estado = estado ?? string.Empty;
        FechaEntrega = fechaEntrega;
        EsUrgente = esUrgente;
        NumeroEntregas = numeroEntregas;
        PuntajeCalificacion = puntajeCalificacion;
    }

    protected override IEnumerable<object?> ObtenerComponentesIgualdad()
    {
        yield return SolicitudId;
        yield return TipoTrabajo;
        yield return AreaTematica;
        yield return Estado;
        yield return FechaEntrega;
        yield return EsUrgente;
        yield return NumeroEntregas;
        yield return PuntajeCalificacion;
    }
}