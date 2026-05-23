using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GrupoXpert.Domain.Academia;
using GrupoXpert.Application.Common.Interfaces;

namespace GrupoXpert.Application.Academia.Commands;

/// <summary>
/// Manejador para que el administrador acepte una postulación a una solicitud.
/// </summary>
public sealed class SeleccionarPostuladoHandler(
    ISolicitudAcademicaRepository repositorio,
    IUnidadDeTrabajo unidadDeTrabajo)
    : IRequestHandler<SeleccionarPostuladoCommand, Unit>
{
    public async Task<Unit> Handle(SeleccionarPostuladoCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await repositorio.ObtenerPorIdAsync(request.SolicitudId, cancellationToken);
        if (solicitud == null)
        {
            throw new InvalidOperationException("La solicitud académica no existe.");
        }

        solicitud.SeleccionarPostulado(request.ColaboradorId);

        await repositorio.ActualizarAsync(solicitud, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
