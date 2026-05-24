using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class AprobarAvanceHandler(
    IAvanceRepository avanceRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<AprobarAvanceCommand, Unit>
{
    public async Task<Unit> Handle(AprobarAvanceCommand comando, CancellationToken cancelacion)
    {
        var avance = await avanceRepositorio.ObtenerPorIdAsync(comando.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance no existe.");

        avance.Aprobar();

        await avanceRepositorio.ActualizarAsync(avance, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return Unit.Value;
    }
}
