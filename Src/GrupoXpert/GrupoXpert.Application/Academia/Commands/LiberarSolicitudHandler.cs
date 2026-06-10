using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class LiberarSolicitudHandler(
    ISolicitudAcademicaRepository solicitudRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<LiberarSolicitudCommand, Unit>
{
    public async Task<Unit> Handle(LiberarSolicitudCommand comando, CancellationToken cancelacion)
    {
        var solicitud = await solicitudRepositorio.ObtenerPorIdAsync(comando.SolicitudId, cancelacion)
            ?? throw new InvalidOperationException("La solicitud no existe.");

        solicitud.Liberar();

        await solicitudRepositorio.ActualizarAsync(solicitud, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return Unit.Value;
    }
}