using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Application.Academia.Commands;

/// <summary>
/// Manejador para procesar la postulación de un asesor a una solicitud.
/// </summary>
public sealed class PostularASolicitudHandler(
    ISolicitudAcademicaRepository repositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<PostularASolicitudCommand, Unit>
{
    public async Task<Unit> Handle(PostularASolicitudCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await repositorio.ObtenerPorIdAsync(request.SolicitudId, cancellationToken);
        if (solicitud == null)
        {
            throw new InvalidOperationException("La solicitud académica no existe.");
        }

        solicitud.PostularAsesor(request.ColaboradorId);

        await repositorio.ActualizarAsync(solicitud, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
