using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Commands;

public sealed class CalificarColaboradorHandler(
    ICalificacionColaboradorRepository calificacionRepositorio,
    ISolicitudAcademicaRepository solicitudRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<CalificarColaboradorCommand, CalificacionColaboradorDto>
{
    public async Task<CalificacionColaboradorDto> Handle(CalificarColaboradorCommand comando, CancellationToken cancelacion)
    {
        var solicitud = await solicitudRepositorio.ObtenerPorIdAsync(comando.SolicitudId, cancelacion)
            ?? throw new InvalidOperationException("La solicitud no existe.");

        if (solicitud.Estado != EstadoSolicitud.Liberacion)
            throw new InvalidOperationException("Solo se puede calificar un colaborador en solicitudes en estado Liberación.");

        if (solicitud.ClienteId != comando.ClienteId)
            throw new InvalidOperationException("Solo el cliente de la solicitud puede calificar al colaborador.");

        var calificacionExistente = await calificacionRepositorio.ObtenerPorSolicitudAsync(comando.SolicitudId, cancelacion);
        if (calificacionExistente is not null)
            throw new InvalidOperationException("Esta solicitud ya tiene una calificación registrada.");

        var calificacion = CalificacionColaborador.Calificar(
            comando.SolicitudId,
            comando.ClienteId,
            comando.ColaboradorId,
            comando.Puntaje,
            comando.Observacion);

        await calificacionRepositorio.AgregarAsync(calificacion, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        string? nombreColaborador = null;
        var perfilColaborador = await perfilColaboradorRepositorio.ObtenerPorIdAsync(comando.ColaboradorId, cancelacion);
        if (perfilColaborador is not null)
        {
            var usuarioColaborador = await usuarioRepositorio.ObtenerPorIdAsync(perfilColaborador.UsuarioId, cancelacion);
            nombreColaborador = usuarioColaborador?.Nombre;
        }

        return new CalificacionColaboradorDto(
            calificacion.Id,
            calificacion.SolicitudId,
            calificacion.ClienteId,
            calificacion.ColaboradorId,
            nombreColaborador,
            calificacion.Puntaje.Valor,
            calificacion.Observacion,
            calificacion.FechaCalificacion);
    }
}