using System;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class AvanceDto
{
    public Guid Id { get; set; }
    public Guid SolicitudId { get; set; }
    public Guid AsesorId { get; set; }
    public string NombreAsesor { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int NumeroFase { get; set; }
    public int Tipo { get; set; }
    public int Estado { get; set; }
    public DateTime FechaSubida { get; set; }
    public System.Collections.Generic.List<ComentarioDto> Comentarios { get; set; } = new();
    public System.Collections.Generic.List<ArchivoAdjuntoDto> ArchivosAdjuntos { get; set; } = new();
}
