using System.ComponentModel.DataAnnotations;

namespace GrupoXpert.Blazor.Models
{
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Rol { get; set; } = string.Empty;
        
        public bool Activo { get; set; } = true;
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public string? Especialidad { get; set; }
        
        public int CargaTrabajo { get; set; } = 0; // Número de solicitudes asignadas actualmente
    }
    
    public static class RolesUsuario
    {
        public static readonly List<string> Opciones = new List<string>
        {
            "Administrador",
            "Supervisor",
            "Colaborador",
            "Editor",
            "Revisor"
        };
        
        public const string Administrador = "Administrador";
        public const string Supervisor = "Supervisor";
        public const string Colaborador = "Colaborador";
        public const string Editor = "Editor";
        public const string Revisor = "Revisor";
    }
}