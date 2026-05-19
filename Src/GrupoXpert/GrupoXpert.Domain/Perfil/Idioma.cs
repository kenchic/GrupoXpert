namespace GrupoXpert.Domain.Perfil;

public class Idioma
{
    public string Nombre { get; private set; }

    private Idioma() { } // Para EF Core

    public Idioma(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El idioma no puede estar vacío.");
            
        Nombre = nombre;
    }
}
