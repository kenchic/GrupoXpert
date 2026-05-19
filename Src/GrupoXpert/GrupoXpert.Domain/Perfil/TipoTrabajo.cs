namespace GrupoXpert.Domain.Perfil;

public class TipoTrabajo
{
    public string Tipo { get; private set; }

    private TipoTrabajo() { } // Para EF Core

    public TipoTrabajo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de trabajo no puede estar vacío.");
            
        Tipo = tipo;
    }
}
