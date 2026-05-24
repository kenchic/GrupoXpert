using GrupoXpert.Domain.Common;
using GrupoXpert.Domain.Exceptions;

namespace GrupoXpert.Domain.Academia;

public sealed class Comentario : Entity
{
    public Guid AvanceId { get; private set; }
    public Guid AutorId { get; private set; }
    public string Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Comentario()
    {
        Contenido = string.Empty;
    }

    public Comentario(Guid avanceId, Guid autorId, string contenido)
    {
        if (avanceId == Guid.Empty)
            throw new ArgumentException("El ID del avance es requerido.", nameof(avanceId));

        if (autorId == Guid.Empty)
            throw new ArgumentException("El ID del autor es requerido.", nameof(autorId));

        if (string.IsNullOrWhiteSpace(contenido))
            throw new ExcepcionDominio("El contenido del comentario no puede estar vacío.");

        AvanceId = avanceId;
        AutorId = autorId;
        Contenido = contenido;
        FechaCreacion = DateTime.UtcNow;
    }
}
