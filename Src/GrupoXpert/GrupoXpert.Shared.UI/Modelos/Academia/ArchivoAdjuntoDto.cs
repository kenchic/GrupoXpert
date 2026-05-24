using System;

namespace GrupoXpert.Shared.UI.Modelos.Academia;

public class ArchivoAdjuntoDto
{
    public Guid Id { get; set; }
    public Guid AvanceId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public string TipoContenido { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
}
