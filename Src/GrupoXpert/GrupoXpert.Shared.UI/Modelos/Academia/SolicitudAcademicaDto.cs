using System;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class SolicitudAcademicaDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public int NivelAcademico { get; set; }
    public int TipoTrabajo { get; set; }
    public string AreaTematica { get; set; } = string.Empty;
    public DateTime FechaEntrega { get; set; }
    public int NumeroPaginasOPalabras { get; set; }
    public int NormaCitacion { get; set; }
    public int Idioma { get; set; }
    public string FormatoRequerido { get; set; } = string.Empty;
    public string MaterialBase { get; set; } = string.Empty;
    public bool EsUrgente { get; set; }
    public bool EntregaPorFases { get; set; }
    public int Estado { get; set; }
    public Guid? AsesorId { get; set; }
    public string? NombreAsesor { get; set; }
}
