using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Queries;

public sealed record ObtenerTodasCalificacionesQuery : IRequest<IReadOnlyList<CalificacionColaboradorDto>>;

public sealed class ObtenerTodasCalificacionesHandler(
    ICalificacionColaboradorRepository calificacionRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerTodasCalificacionesQuery, IReadOnlyList<CalificacionColaboradorDto>>
{
    public async Task<IReadOnlyList<CalificacionColaboradorDto>> Handle(ObtenerTodasCalificacionesQuery consulta, CancellationToken cancelacion)
    {
        var calificaciones = await calificacionRepositorio.ObtenerTodasAsync(cancelacion);

        var resultado = new List<CalificacionColaboradorDto>();
        foreach (var cal in calificaciones)
        {
            string? nombreColaborador = null;
            var perfilColaborador = await perfilColaboradorRepositorio.ObtenerPorIdAsync(cal.ColaboradorId, cancelacion);
            if (perfilColaborador is not null)
            {
                var usuarioColaborador = await usuarioRepositorio.ObtenerPorIdAsync(perfilColaborador.UsuarioId, cancelacion);
                nombreColaborador = usuarioColaborador?.Nombre;
            }

            resultado.Add(new CalificacionColaboradorDto(
                cal.Id,
                cal.SolicitudId,
                cal.ClienteId,
                cal.ColaboradorId,
                nombreColaborador,
                cal.Puntaje.Valor,
                cal.Observacion,
                cal.FechaCalificacion));
        }

        return resultado.AsReadOnly();
    }
}