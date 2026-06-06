using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Calidad.Queries;

public sealed class ObtenerRevisionPorAvanceHandler(
    IRevisionCalidadRepository revisionRepositorio,
    IUsuarioRepository usuarioRepositorio)
    : IRequestHandler<ObtenerRevisionPorAvanceQuery, RevisionCalidadDto?>
{
    public async Task<RevisionCalidadDto?> Handle(ObtenerRevisionPorAvanceQuery consulta, CancellationToken cancelacion)
    {
        var revision = await revisionRepositorio.ObtenerPorAvanceAsync(consulta.AvanceId, cancelacion);
        if (revision is null) return null;

        var revisor = await usuarioRepositorio.ObtenerPorIdAsync(revision.RevisorId, cancelacion);

        return new RevisionCalidadDto(
            revision.Id,
            revision.AvanceId,
            revision.RevisorId,
            revisor?.Nombre,
            revision.FechaRevision,
            revision.VistoBueno,
            revision.Observaciones,
            (int)revision.Estado);
    }
}
