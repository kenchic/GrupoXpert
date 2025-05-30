using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace GrupoXpert.Blazor.Models
{
    public class SolicitudAcademica
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nivel académico es requerido")]
        public string NivelAcademico { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tipo de trabajo es requerido")]
        public string TipoTrabajo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tema es requerido")]
        [StringLength(500, ErrorMessage = "El tema no puede exceder 500 caracteres")]
        public string Tema { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El área del conocimiento es requerida")]
        public string AreaConocimiento { get; set; } = string.Empty;
        
        [Range(1, 10000, ErrorMessage = "El número de páginas debe estar entre 1 y 10000")]
        public int? NumeroPaginas { get; set; }
        
        [Range(100, 1000000, ErrorMessage = "El número de palabras debe estar entre 100 y 1,000,000")]
        public int? NumeroPalabras { get; set; }
        
        [Required(ErrorMessage = "La fecha de entrega es requerida")]
        public DateTime? FechaEntrega { get; set; }
        
        public string Instrucciones { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La norma de citación es requerida")]
        public string NormaCitacion { get; set; } = string.Empty;
        
        public bool EsUrgente { get; set; }
        
        public bool EntregaPorFases { get; set; }
        
        public List<IBrowserFile> DocumentosAdjuntos { get; set; } = new List<IBrowserFile>();
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public string Estado { get; set; } = "Pendiente";
    }
    
    public static class NivelesAcademicos
    {
        public static readonly List<string> Opciones = new List<string>
        {
            "Pregrado",
            "Especialización",
            "Maestría",
            "Doctorado"
        };
    }
    
    public static class TiposTrabajos
    {
        public static readonly List<string> Opciones = new List<string>
        {
            "Ensayo",
            "Artículo",
            "Tesis",
            "Presentación",
            "Monografía",
            "Proyecto de grado",
            "Informe de investigación",
            "Reseña",
            "Caso de estudio",
            "Propuesta de investigación"
        };
    }
    
    public static class NormasCitacion
    {
        public static readonly List<string> Opciones = new List<string>
        {
            "APA 7ma edición",
            "IEEE",
            "MLA",
            "Chicago",
            "Harvard",
            "Vancouver",
            "ICONTEC",
            "Otra (especificar en instrucciones)"
        };
    }
    
    public static class AreasConocimiento
    {
        public static readonly List<string> Opciones = new List<string>
        {
            "Administración y Negocios",
            "Ciencias de la Salud",
            "Ciencias Exactas y Naturales",
            "Ciencias Sociales",
            "Derecho",
            "Educación",
            "Humanidades",
            "Ingeniería",
            "Psicología",
            "Tecnología",
            "Arte y Diseño",
            "Comunicación",
            "Economía",
            "Filosofía",
            "Historia",
            "Idiomas",
            "Matemáticas",
            "Medicina",
            "Otra (especificar en tema)"
        };
    }
}