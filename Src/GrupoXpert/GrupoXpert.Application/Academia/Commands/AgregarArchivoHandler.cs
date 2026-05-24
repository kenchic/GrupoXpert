using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class AgregarArchivoHandler(
    IAvanceRepository avanceRepositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<AgregarArchivoCommand, Unit>
{
    public async Task<Unit> Handle(AgregarArchivoCommand comando, CancellationToken cancelacion)
    {
        var avance = await avanceRepositorio.ObtenerPorIdAsync(comando.AvanceId, cancelacion)
            ?? throw new InvalidOperationException("El avance no existe.");

        avance.AgregarArchivo(
            comando.NombreArchivo,
            comando.Url,
            comando.TamanioBytes,
            comando.TipoContenido);

        await avanceRepositorio.ActualizarAsync(avance, cancelacion);
        await unidadDeTrabajo.GuardarCambiosAsync(cancelacion);

        return Unit.Value;
    }
}
