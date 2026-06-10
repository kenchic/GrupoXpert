using GrupoXpert.Domain.Calificacion.Events;
using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Calificacion;

public class CalificacionColaborador : AggregateRoot
{
    public Guid SolicitudId { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid ColaboradorId { get; private set; }
    public Puntaje Puntaje { get; private set; }
    public string? Observacion { get; private set; }
    public DateTime FechaCalificacion { get; private set; }

    private CalificacionColaborador()
    {
        Puntaje = null!;
    }

    private CalificacionColaborador(
        Guid solicitudId,
        Guid clienteId,
        Guid colaboradorId,
        Puntaje puntaje,
        string? observacion)
    {
        SolicitudId = solicitudId;
        ClienteId = clienteId;
        ColaboradorId = colaboradorId;
        Puntaje = puntaje;
        Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();
        FechaCalificacion = DateTime.UtcNow;
    }

    public static CalificacionColaborador Calificar(
        Guid solicitudId,
        Guid clienteId,
        Guid colaboradorId,
        int puntaje,
        string? observacion)
    {
        if (solicitudId == Guid.Empty)
            throw new ArgumentException("El ID de la solicitud es requerido.", nameof(solicitudId));

        if (clienteId == Guid.Empty)
            throw new ArgumentException("El ID del cliente es requerido.", nameof(clienteId));

        if (colaboradorId == Guid.Empty)
            throw new ArgumentException("El ID del colaborador es requerido.", nameof(colaboradorId));

        if (clienteId == colaboradorId)
            throw new ExcepcionDominio("Un cliente no puede calificarse a sí mismo.");

        var puntajeVo = new Puntaje(puntaje);

        var calificacion = new CalificacionColaborador(
            solicitudId, clienteId, colaboradorId, puntajeVo, observacion);

        calificacion.AgregarEventoDominio(new ColaboradorCalificadoEvent(
            calificacion.Id, solicitudId, clienteId, colaboradorId, puntaje));

        return calificacion;
    }

    public void ActualizarObservacion(string observacion)
    {
        Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();
    }
}