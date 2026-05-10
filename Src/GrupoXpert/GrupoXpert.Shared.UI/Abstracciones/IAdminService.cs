using GrupoXpert.Shared.UI.Modelos;

namespace GrupoXpert.Shared.UI.Abstracciones;

public interface IAdminService
{
    Task<ResultadoPaginadoDto<UsuarioListaDto>> ObtenerUsuariosPaginadoAsync(
        int? tipo = null, 
        bool? estaAprobado = null,
        int? estadoVerificacion = null,
        int pagina = 1, 
        int tamanoPagina = 20);

    Task<bool> AprobarColaboradorAsync(Guid colaboradorId);
    
    Task<bool> RevocarAprobacionAsync(Guid colaboradorId);

    Task<bool> ValidarPerfilColaboradorAsync(Guid colaboradorId);
}
