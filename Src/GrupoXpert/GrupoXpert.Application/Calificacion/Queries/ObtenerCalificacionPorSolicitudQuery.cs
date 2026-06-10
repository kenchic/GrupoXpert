using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Queries;

public sealed record ObtenerCalificacionPorSolicitudQuery(Guid SolicitudId) : IRequest<CalificacionColaboradorDto?>;

public sealed class ObtenerCalificacionPorSolicitudHandler(
    ICalificacionColaboradorRepository calificacionRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerCalificacionPorSolicitudQuery, CalificacionColaboradorDto?>
{
    public async Task<CalificacionColaboradorDto?> Handle(ObtenerCalificacionPorSolicitudQuery consulta, CancellationToken cancelacion)
    {
        var calificacion = await calificacionRepositorio.ObtenerPorSolicitudAsync(consulta.SolicitudId, cancelacion);
        if (calificacion is null) return null;

        string? nombreColaborador = null;
        var perfilColaborador = await perfilColaboradorRepositorio.ObtenerPorIdAsync(calificacion.ColaboradorId, cancelacion);
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