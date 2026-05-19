namespace GrupoXpert.Domain.Perfil;

public class AreaConocimiento
{
    public string Area { get; private set; }

    private AreaConocimiento() { } // Para EF Core

    public AreaConocimiento(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            throw new ArgumentException("El área de conocimiento no puede estar vacía.");
            
        Area = area;
    }
}
