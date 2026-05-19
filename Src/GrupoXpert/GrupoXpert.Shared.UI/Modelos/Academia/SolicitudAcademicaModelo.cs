using System.ComponentModel.DataAnnotations;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class SolicitudAcademicaModelo
{
    [Required(ErrorMessage = "El nivel académico es obligatorio.")]
    public int NivelAcademico { get; set; } = 1; // Default: Pregrado

    [Required(ErrorMessage = "El tipo de trabajo es obligatorio.")]
    public int TipoTrabajo { get; set; } = 0; // Default: Ensayo

    [Required(ErrorMessage = "El área temática es obligatoria.")]
    [StringLength(200, ErrorMessage = "El área temática no puede superar los 200 caracteres.")]
    public string AreaTematica { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad de páginas o palabras es obligatoria.")]
    [Range(1, 100000, ErrorMessage = "Debe ser al menos 1 página o palabra.")]
    public int NumeroPaginasOPalabras { get; set; } = 1;

    [Required(ErrorMessage = "La fecha de entrega es obligatoria.")]
    public DateTime? FechaEntrega { get; set; } = DateTime.Today.AddDays(7); // Default: 7 días en el futuro

    [Required(ErrorMessage = "La norma de citación es obligatoria.")]
    public int NormaCitacion { get; set; } = 0; // Default: APA

    [Required(ErrorMessage = "El idioma es obligatorio.")]
    public int Idioma { get; set; } = 0; // Default: Español

    [Required(ErrorMessage = "El formato requerido es obligatorio.")]
    [StringLength(500, ErrorMessage = "El formato no puede superar los 500 caracteres.")]
    public string FormatoRequerido { get; set; } = "Normas estándar de presentación, letra Arial 12, interlineado doble.";

    [Required(ErrorMessage = "Las instrucciones o material base son obligatorios.")]
    public string MaterialBase { get; set; } = string.Empty;

    public bool EsUrgente { get; set; }
    public bool EntregaPorFases { get; set; }
}
