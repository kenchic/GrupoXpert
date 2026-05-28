namespace GrupoXpert.Shared.UI.Modelos.Academia;

public sealed class ArchivoSubidaModelo
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public long TamanioBytes { get; set; }
    public Stream Contenido { get; set; } = Stream.Null;
}
