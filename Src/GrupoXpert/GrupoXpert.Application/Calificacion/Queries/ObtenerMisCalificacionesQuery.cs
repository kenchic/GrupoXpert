using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Queries;

public sealed record ObtenerMisCalificacionesQuery(Guid UsuarioId) : IRequest<IReadOnlyList<CalificacionColaboradorDto>>;

public sealed class ObtenerMisCalificacionesHandler(
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    ICalificacionColaboradorRepository calificacionRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerMisCalificacionesQuery, IReadOnlyList<CalificacionColaboradorDto>>
{
    public async Task<IReadOnlyList<CalificacionColaboradorDto>> Handle(ObtenerMisCalificacionesQuery consulta, CancellationToken cancelacion)
    {
        var perfilColaborador = await perfilColaboradorRepositorio.ObtenerPorUsuarioIdAsync(consulta.UsuarioId, cancelacion)
            ?? throw new InvalidOperationException("No se encontró el perfil de colaborador para el usuario autenticado.");

        var calificaciones = await calificacionRepositorio.ObtenerPorColaboradorAsync(perfilColaborador.Id, cancelacion);

        var resultado = new List<CalificacionColaboradorDto>();
        foreach (var cal in calificaciones)
        {
            var usuarioColaborador = await usuarioRepositorio.ObtenerPorIdAsync(perfilColaborador.UsuarioId, cancelacion);

            resultado.Add(new CalificacionColaboradorDto(
                cal.Id,
                cal.SolicitudId,
                cal.ClienteId,
                cal.ColaboradorId,
                usuarioColaborador?.Nombre,
                cal.Puntaje.Valor,
                cal.Observacion,
                cal.FechaCalificacion));
        }

        return resultado.AsReadOnly();
    }
}