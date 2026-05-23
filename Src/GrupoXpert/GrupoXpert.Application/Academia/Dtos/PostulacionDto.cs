using System;

namespace GrupoXpert.Application.Academia.Dtos;

/// <summary>
/// DTO que representa una postulación de asesor.
/// </summary>
public record PostulacionDto(
    Guid Id,
    Guid ColaboradorId,
    string NombreColaborador,
    decimal EvaluacionCalidad,
    DateTime FechaPostulacion,
    int Estado
);
