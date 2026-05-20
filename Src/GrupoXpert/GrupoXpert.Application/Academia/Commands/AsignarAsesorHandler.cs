using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class AsignarAsesorHandler(
    ISolicitudAcademicaRepository repositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<AsignarAsesorCommand, Unit>
{
    public async Task<Unit> Handle(AsignarAsesorCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await repositorio.ObtenerPorIdAsync(request.SolicitudId, cancellationToken);
        if (solicitud == null)
        {
            throw new InvalidOperationException("La solicitud académica no existe.");
        }

        solicitud.AsignarAsesor(request.AsesorId);

        await repositorio.ActualizarAsync(solicitud, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
