using GrupoXpert.Application.Academia.Dtos;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Academia;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public sealed class CrearSolicitudAcademicaHandler(
    ISolicitudAcademicaRepository repositorio,
    IUnidadDeTrabajo unidadDeTrabajo) 
    : IRequestHandler<CrearSolicitudAcademicaCommand, SolicitudAcademicaDto>
{
    public async Task<SolicitudAcademicaDto> Handle(CrearSolicitudAcademicaCommand request, CancellationToken cancellationToken)
    {
        var solicitud = SolicitudAcademica.Crear(
            request.ClienteId,
            (NivelAcademico)request.NivelAcademico,
            (TipoTrabajo)request.TipoTrabajo,
            request.AreaTematica,
            request.FechaEntrega,
            request.NumeroPaginasOPalabras,
            (NormaCitacion)request.NormaCitacion,
            (IdiomaRequerido)request.Idioma,
            request.FormatoRequerido,
            request.MaterialBase,
            request.EsUrgente,
            request.EntregaPorFases
        );

        await repositorio.AgregarAsync(solicitud, cancellationToken);
        await unidadDeTrabajo.GuardarCambiosAsync(cancellationToken);

        return new SolicitudAcademicaDto(
            solicitud.Id,
            solicitud.ClienteId,
            (int)solicitud.NivelAcademico,
            (int)solicitud.TipoTrabajo,
            solicitud.AreaTematica,
            solicitud.FechaEntrega,
            solicitud.NumeroPaginasOPalabras,
            (int)solicitud.NormaCitacion,
            (int)solicitud.Idioma,
            solicitud.FormatoRequerido,
            solicitud.MaterialBase,
            solicitud.EsUrgente,
            solicitud.EntregaPorFases
        );
    }
}
