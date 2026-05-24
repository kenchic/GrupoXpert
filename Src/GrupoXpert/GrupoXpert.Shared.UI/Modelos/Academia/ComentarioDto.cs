using System;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class ComentarioDto
{
    public Guid Id { get; set; }
    public Guid AvanceId { get; set; }
    public Guid AutorId { get; set; }
    public string NombreAutor { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
