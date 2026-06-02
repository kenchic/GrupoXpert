namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IDescargadorArchivos
{
    Task DescargarAsync(string urlAbsoluta);
}
