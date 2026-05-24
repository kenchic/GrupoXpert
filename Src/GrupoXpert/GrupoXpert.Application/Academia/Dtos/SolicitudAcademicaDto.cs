using System;
using System.Collections.Generic;

namespace GrupoXpert.Application.Academia.Dtos;

public record SolicitudAcademicaDto(
    Guid Id,
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
    bool EntregaPorFases,
    int Estado,
    Guid? AsesorId,
    string? NombreAsesor = null,
    IReadOnlyList<PostulacionDto>? Postulaciones = null
);
