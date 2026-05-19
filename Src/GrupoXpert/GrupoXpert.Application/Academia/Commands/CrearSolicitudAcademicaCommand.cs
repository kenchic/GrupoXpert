using GrupoXpert.Application.Academia.Dtos;
using MediatR;

namespace GrupoXpert.Application.Academia.Commands;

public record CrearSolicitudAcademicaCommand(
    Guid ClienteId,
    int NivelAcademico,
    int TipoTrabajo,
    string AreaTematica,
    DateTime FechaEntrega,
    int NumeroPaginasOPalabras,
    int NormaCitacion,
    int Idioma,
    string FormatoRequerido,
    string MaterialBase,
    bool EsUrgente,
    bool EntregaPorFases
) : IRequest<SolicitudAcademicaDto>;
