using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class RechazarAvanceHandler(
    IAvanceRepository avanceRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<RechazarAvanceCommand, Unit>
{
    public async Task<Unit> Handle(RechazarAvanceCommand comando, CancellationToken cancelacion)
    {
        var avance = await avanceRepositorio.ObtenerPorIdAsync(comando.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance no existe.");

        avance.Rechazar();

        await avanceRepositorio.ActualizarAsync(avance, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return Unit.Value;
    }
}
