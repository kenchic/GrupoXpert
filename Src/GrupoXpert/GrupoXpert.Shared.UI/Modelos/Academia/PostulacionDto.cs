using System;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class PostulacionDto
{
    public Guid Id { get; set; }
    public Guid ColaboradorId { get; set; }
    public string NombreColaborador { get; set; } = string.Empty;
    public decimal EvaluacionCalidad { get; set; }
    public DateTime FechaPostulacion { get; set; }
    public int Estado { get; set; }
}
