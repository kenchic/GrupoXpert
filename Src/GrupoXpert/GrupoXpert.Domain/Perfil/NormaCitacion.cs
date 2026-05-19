namespace GrupoXpert.Domain.Perfil;

public class NormaCitacion
{
    public string Norma { get; private set; }

    private NormaCitacion() { } // Para EF Core

    public NormaCitacion(string norma)
    {
        if (string.IsNullOrWhiteSpace(norma))
            throw new ArgumentException("La norma de citación no puede estar vacía.");
            
        Norma = norma;
    }
}
