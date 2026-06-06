using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed class RechazarRevisionHandler(
    IRevisionCalidadRepository revisionRepositorio,
    IUsuarioRepository usuarioRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<RechazarRevisionCommand, RevisionCalidadDto>
{
    public async Task<RevisionCalidadDto> Handle(RechazarRevisionCommand comando, CancellationToken cancelacion)
    {
        var revision = await revisionRepositorio.ObtenerPorIdAsync(comando.RevisionId, cancelacion)
            ?? throw new InvalidOperationException("La revisión de calidad no existe.");

        revision.Rechazar(comando.Observaciones);
        await revisionRepositorio.ActualizarAsync(revision, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

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
