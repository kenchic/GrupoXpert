using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Calidad.Queries;

public sealed class ObtenerRevisionesPendientesHandler(
    IRevisionCalidadRepository revisionRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerRevisionesPendientesQuery, IReadOnlyList<RevisionCalidadDto>>
{
    public async Task<IReadOnlyList<RevisionCalidadDto>> Handle(ObtenerRevisionesPendientesQuery consulta, CancellationToken cancelacion)
    {
        var revisiones = await revisionRepositorio.ObtenerPendientesAsync(cancelacion);
        var resultado = new List<RevisionCalidadDto>(revisiones.Count);

        foreach (var revision in revisiones)
        {
            var revisor = await usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, cancelacion);
            resultado.Add(new RevisionCalidadDto(
                revision.Id,
                revision.AvanceId,
                revision.RevisorId,
                revisor?.Nombre,
                revision.FechaRevision,
                revision.VistoBueno,
                revision.Observaciones,
                (int)revision.Estado));
        }

        return resultado;
    }
}
