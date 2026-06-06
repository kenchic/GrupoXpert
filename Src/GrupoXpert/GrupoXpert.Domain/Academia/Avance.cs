using GrupoXpert.Domain.Academia.Events;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Academia;

public class Avance : AggregateRoot
{
    public Guid SolicitudId { get; private set; }
    public Guid AsesorId { get; private set; }
    public string Descripcion { get; private set; }
    public int NumeroFase { get; private set; }
    public TipoAvance Tipo { get; private set; }
    public EstadoAvance Estado { get; private set; }
    public DateTime FechaSubida { get; private set; }

    private readonly List<Comentario> _comentarios = new();
    public IReadOnlyCollection<Comentario> Comentarios => _comentarios.AsReadOnly();

    private readonly List<ArchivoAdjunto> _archivosAdjuntos = new();
    public IReadOnlyCollection<ArchivoAdjunto> ArchivosAdjuntos => _archivosAdjuntos.AsReadOnly();

    private Avance()
    {
        Descripcion = string.Empty;
    }

    private Avance(
        Guid solicitudId,
        Guid asesorId,
        string descripcion,
        int numeroFase,
        TipoAvance tipo)
    {
        SolicitudId = solicitudId;
        AsesorId = asesorId;
        Descripcion = descripcion;
        NumeroFase = numeroFase;
        Tipo = tipo;
        Estado = EstadoAvance.PendienteRevision;
        FechaSubida = DateTime.UtcNow;
    }

    public static Avance Subir(
        Guid solicitudId,
        Guid asesorId,
        string descripcion,
        int numeroFase,
        TipoAvance tipo)
    {
        if (solicitudId == Guid.Empty)
            throw new ArgumentException("El ID de la solicitud es requerido.", nameof(solicitudId));

        if (asesorId == Guid.Empty)
            throw new ArgumentException("El ID del asesor es requerido.", nameof(asesorId));

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ExcepcionDominio("La descripción del avance no puede estar vacía.");

        if (numeroFase < 1)
            throw new ExcepcionDominio("El número de fase debe ser mayor o igual a 1.");

        var avance = new Avance(solicitudId, asesorId, descripcion, numeroFase, tipo);
        avance.AgregarEventoDominio(new AvanceSubidoDomainEvent(avance.Id, solicitudId, asesorId));

        return avance;
    }

    public Comentario AgregarComentario(Guid autorId, string contenido)
    {
        if (autorId == Guid.Empty)
            throw new ArgumentException("El ID del autor es requerido.", nameof(autorId));

        if (string.IsNullOrWhiteSpace(contenido))
            throw new ExcepcionDominio("El contenido del comentario no puede estar vacío.");

        var comentario = new Comentario(Id, autorId, contenido);
        _comentarios.Add(comentario);

        AgregarEventoDominio(new ComentarioAgregadoDomainEvent(Id, comentario.Id, autorId));

        return comentario;
    }

    public ArchivoAdjunto AgregarArchivo(
        string nombreArchivo,
        string url,
        long tamanioBytes,
        string tipoContenido)
    {
        var archivo = new ArchivoAdjunto(Id, nombreArchivo, url, tamanioBytes, tipoContenido);
        _archivosAdjuntos.Add(archivo);

        return archivo;
    }

    public void Aprobar()
    {
        if (Estado != EstadoAvance.PendienteRevision)
            throw new ExcepcionDominio("Solo se pueden aprobar avances pendientes de revisión.");

        Estado = EstadoAvance.Aprobado;
    }

    public void Rechazar()
    {
        if (Estado != EstadoAvance.PendienteRevision)
            throw new ExcepcionDominio("Solo se pueden rechazar avances pendientes de revisión.");

        Estado = EstadoAvance.Rechazado;
    }

    public void Liberar()
    {
        if (Tipo != TipoAvance.Final)
            throw new ExcepcionDominio("Solo se pueden liberar avances de tipo final.");

        if (Estado != EstadoAvance.PendienteRevision && Estado != EstadoAvance.Aprobado)
            throw new ExcepcionDominio("Solo se pueden liberar avances pendientes de revisión o aprobados.");

        Estado = EstadoAvance.Liberado;
    }
}
