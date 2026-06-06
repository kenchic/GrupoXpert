using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed class CrearRevisionCalidadHandler(
    IRevisionCalidadRepository revisionRepositorio,
    IAvanceRepository avanceRepositorio,
    IUsuarioRepository usuarioRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<CrearRevisionCalidadCommand, RevisionCalidadDto>
{
    public async Task<RevisionCalidadDto> Handle(CrearRevisionCalidadCommand comando, CancellationToken cancelacion)
    {
        var avance = await avanceRepositorio.ObtenerPorIdAsync(comando.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance no existe.");

        if (avance.Tipo != TipoAvance.Final)
            throw new InvalidOperationException("Solo se pueden revisar avances de tipo final.");

        var revisionExistente = await revisionRepositorio.ObtenerPorAvanceAsync(comando.AvanceId, cancelacion);
        if (revisionExistente is not null)
            throw new InvalidOperationException("Ya existe una revisión de calidad para este avance.");

        var revision = RevisionCalidad.Crear(comando.AvanceId, comando.RevisorId);
        await revisionRepositorio.AgregarAsync(revision, cancelacion);
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
