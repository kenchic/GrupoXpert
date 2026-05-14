namespace GrupoXpert.Shared.UI.Modelos.Perfil;

public class PerfilClienteModelo
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public int NivelAcademico { get; set; }
    public int UrgenciaEntrega { get; set; }
    public string? Telefono { get; set; }
    public List<string> AreasInteres { get; set; } = new();
}
