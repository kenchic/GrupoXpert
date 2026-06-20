using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IDashboardColaboradorService
{
    Task<DashboardColaboradorModelo?> ObtenerDashboardAsync();
}