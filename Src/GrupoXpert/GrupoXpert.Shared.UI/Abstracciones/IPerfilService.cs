using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IPerfilService
{
    Task<PerfilClienteModelo?> ObtenerPerfilActualAsync();
    Task<bool> ActualizarPerfilAsync(PerfilClienteModelo modelo);
}
