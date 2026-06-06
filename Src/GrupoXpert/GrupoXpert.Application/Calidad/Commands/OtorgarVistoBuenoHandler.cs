using GrupoXpert.Application.Calidad.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Domain.Calidad;
using GrupoXpert.Domain.Identidad;
using MediatR;

namespace GrupoXpert.Application.Calidad.Commands;

public sealed class OtorgarVistoBuenoHandler(
    IRevisionCalidadRepository revisionRepositorio,
    IAvanceRepository avanceRepositorio,
    IUsuarioRepository usuarioRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<OtorgarVistoBuenoCommand, RevisionCalidadDto>
{
    public async Task<RevisionCalidadDto> Handle(OtorgarVistoBuenoCommand comando, CancellationToken cancelacion)
    {
        var revision = await revisionRepositorio.ObtenerPorIdAsync(comando.RevisionId, cancelacion)
            ?? throw new InvalidOperationException("La revisión de calidad no existe.");

        revision.OtorgarVistoBueno();
        await revisionRepositorio.ActualizarAsync(revision, cancelacion);

        var avance = await avanceRepositorio.ObtenerPorIdAsync(revision.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance asociado no existe.");

        avance.Liberar();
        await avanceRepositorio.ActualizarAsync(avance, cancelacion);

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
