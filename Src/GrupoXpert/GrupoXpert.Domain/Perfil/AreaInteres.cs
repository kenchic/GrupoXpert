namespace GrupoXpert.Domain.Perfil;

public class AreaInteres
{
    public string Area { get; private set; }

    private AreaInteres() { } // Para EF Core

    public AreaInteres(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            throw new ArgumentException("El área de interés no puede estar vacía.");
            
        Area = area;
    }
}
