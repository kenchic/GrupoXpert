using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IPerfilColaboradorService
{
    Task<PerfilColaboradorModelo?> ObtenerPerfilColaboradorAsync(Guid usuarioId);
    Task<bool> ActualizarPerfilColaboradorAsync(PerfilColaboradorModelo modelo);
}
