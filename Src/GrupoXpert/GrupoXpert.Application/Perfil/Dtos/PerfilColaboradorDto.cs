using System;
using System.Collections.Generic;

namespace GrupoXpert.Application.Perfil.Dtos;

public class PerfilColaboradorDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public bool ValidadoPorAdmin { get; set; }
    public int NivelAcademico { get; set; }
    public int DisponibilidadHorasSemana { get; set; }
    public int CargaAcademicaIdeal { get; set; }
    public decimal EvaluacionCalidad { get; set; }
    
    public List<string> AreasConocimiento { get; set; } = new();
    public List<string> TiposTrabajo { get; set; } = new();
    public List<string> Idiomas { get; set; } = new();
    public List<string> NormasCitacion { get; set; } = new();
}
