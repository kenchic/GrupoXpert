using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class AgregarComentarioHandler(
    IAvanceRepository avanceRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<AgregarComentarioCommand, Unit>
{
    public async Task<Unit> Handle(AgregarComentarioCommand comando, CancellationToken cancelacion)
    {
        var avance = await avanceRepositorio.ObtenerPorIdAsync(comando.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance no existe.");

        avance.AgregarComentario(comando.AutorId, comando.Contenido);

        await avanceRepositorio.ActualizarAsync(avance, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return Unit.Value;
    }
}
