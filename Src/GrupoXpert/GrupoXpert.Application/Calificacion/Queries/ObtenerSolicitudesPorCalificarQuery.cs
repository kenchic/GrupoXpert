using GrupoXpert.Application.Calificacion.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calificacion;
using GrupoXpert.Domain.Identidad;
using GrupoXpert.Domain.Perfil;
using MediatR;

namespace GrupoXpert.Application.Calificacion.Queries;

public sealed record ObtenerSolicitudesPorCalificarQuery(Guid UsuarioId) : IRequest<IReadOnlyList<SolicitudPorCalificarDto>>;

public sealed class ObtenerSolicitudesPorCalificarHandler(
    ISolicitudAcademicaRepository solicitudRepositorio,
    ICalificacionColaboradorRepository calificacionRepositorio,
    IPerfilClienteRepository perfilClienteRepositorio,
    IPerfilColaboradorRepository perfilColaboradorRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerSolicitudesPorCalificarQuery, IReadOnlyList<SolicitudPorCalificarDto>>
{
    public async Task<IReadOnlyList<SolicitudPorCalificarDto>> Handle(
        ObtenerSolicitudesPorCalificarQuery consulta,
        CancellationToken cancelacion)
    {
        var perfil = await perfilClienteRepositorio.ObtenerPorUsuarioIdAsync(consulta.UsuarioId, cancelacion)
            ?? throw new InvalidOperationException("No se encontró el perfil del cliente.");

        var solicitudes = await solicitudRepositorio.ObtenerEnLiberacionPorClienteAsync(perfil.Id, cancelacion);

        var resultado = new List<SolicitudPorCalificarDto>();
        foreach (var solicitud in solicitudes)
        {
            string? nombreAsesor = null;
            if (solicitud.AsesorId.HasValue)
            {
                var perfilColab = await perfilColaboradorRepositorio.ObtenerPorIdAsync(solicitud.AsesorId.Value, cancelacion);
                if (perfilColab is not null)
                {
                    var usuarioAsesor = await usuarioRepositorio.ObtenerPorIdAsync(perfilColab.UsuarioId, cancelacion);
                    nombreAsesor = usuarioAsesor?.Nombre;
                }
            }

            var calificacion = await calificacionRepositorio.ObtenerPorSolicitudAsync(solicitud.Id, cancelacion);

            resultado.Add(new SolicitudPorCalificarDto(
                solicitud.Id,
                solicitud.ClienteId,
                solicitud.AreaTematica,
                (int)solicitud.Estado,
                solicitud.AsesorId,
                nombreAsesor,
                calificacion is not null,
                calificacion?.Puntaje.Valor,
                calificacion?.Observacion,
                calificacion?.FechaCalificacion));
        }

        return resultado.AsReadOnly();
    }
}