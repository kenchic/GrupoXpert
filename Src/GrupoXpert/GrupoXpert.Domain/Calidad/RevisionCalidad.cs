using GrupoXpert.Domain.Calidad.Events;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Calidad;

public class RevisionCalidad : AggregateRoot
{
    public Guid AvanceId { get; private set; }
    public Guid RevisorId { get; private set; }
    public DateTime FechaRevision { get; private set; }
    public bool VistoBueno { get; private set; }
    public string? Observaciones { get; private set; }
    public EstadoRevisionCalidad Estado { get; private set; }

    private RevisionCalidad()
    {
    }

    private RevisionCalidad(Guid avanceId, Guid revisorId)
    {
        AvanceId = avanceId;
        RevisorId = revisorId;
        FechaRevision = DateTime.UtcNow;
        VistoBueno = false;
        Observaciones = null;
        Estado = EstadoRevisionCalidad.Pendiente;
    }

    public static RevisionCalidad Crear(Guid avanceId, Guid revisorId)
    {
        if (avanceId == Guid.Empty)
            throw new ArgumentException("El ID del avance es requerido.", nameof(avanceId));

        if (revisorId == Guid.Empty)
            throw new ArgumentException("El ID del revisor es requerido.", nameof(revisorId));

        var revision = new RevisionCalidad(avanceId, revisorId);
        revision.AgregarEventoDominio(new RevisionCalidadCreadaEvent(revision.Id, avanceId, revisorId));

        return revision;
    }

    public void OtorgarVistoBueno()
    {
        if (Estado != EstadoRevisionCalidad.Pendiente)
            throw new ExcepcionDominio("Solo se puede otorgar visto bueno a revisiones pendientes.");

        VistoBueno = true;
        Estado = EstadoRevisionCalidad.Aprobado;
        FechaRevision = DateTime.UtcNow;

        AgregarEventoDominio(new VistoBuenoOtorgadoEvent(Id, AvanceId, RevisorId));
    }

    public void Rechazar(string observaciones)
    {
        if (Estado != EstadoRevisionCalidad.Pendiente)
            throw new ExcepcionDominio("Solo se pueden rechazar revisiones pendientes.");

        if (string.IsNullOrWhiteSpace(observaciones))
            throw new ExcepcionDominio("Las observaciones son requeridas al rechazar una revisión.");

        VistoBueno = false;
        Observaciones = observaciones.Trim();
        Estado = EstadoRevisionCalidad.Rechazado;
        FechaRevision = DateTime.UtcNow;

        AgregarEventoDominio(new RevisionRechazadaEvent(Id, AvanceId, RevisorId, observaciones.Trim()));
    }
}
