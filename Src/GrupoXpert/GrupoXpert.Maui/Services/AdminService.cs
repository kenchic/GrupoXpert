using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos;
using Microsoft.Maui.Storage;

namespace GrupoXpert.Maui.Services;

public sealed class AdminService(HttpClient http) : IAdminService
{
    private readonly HttpClient _http = http;

    private void PrepararCliente()
    {
        var token = Preferences.Default.Get("authToken", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<ResultadoPaginadoDto<UsuarioListaDto>> ObtenerUsuariosPaginadoAsync(
        int? tipo = null,
        bool? estaAprobado = null,
        int? estadoVerificacion = null,
        int pagina = 1,
        int tamanoPagina = 20)
    {
        PrepararCliente();
        var queryString = $"?pagina={pagina}&tamanoPagina={tamanoPagina}";

        if (tipo.HasValue)
        {
            queryString += $"&tipo={tipo.Value}";
        }

        if (estaAprobado.HasValue)
        {
            queryString += $"&estaAprobado={estaAprobado.Value.ToString().ToLower()}";
        }

        if (estadoVerificacion.HasValue)
        {
            queryString += $"&estadoVerificacion={estadoVerificacion.Value}";
        }

        try
        {
            var response = await _http.GetFromJsonAsync<ResultadoPaginadoDto<UsuarioListaDto>>($"api/admin/usuarios{queryString}");

            return response ?? new ResultadoPaginadoDto<UsuarioListaDto>
            {
                Elementos = Array.Empty<UsuarioListaDto>(),
                Pagina = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = 0
            };
        }
        catch
        {
            return new ResultadoPaginadoDto<UsuarioListaDto>
            {
                Elementos = Array.Empty<UsuarioListaDto>(),
                Pagina = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = 0
            };
        }
    }

    public async Task<bool> AprobarColaboradorAsync(Guid colaboradorId)
    {
        PrepararCliente();
        var response = await _http.PostAsync($"api/admin/colaboradores/{colaboradorId}/aprobar", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RevocarAprobacionAsync(Guid colaboradorId)
    {
        PrepararCliente();
        var response = await _http.PostAsync($"api/admin/colaboradores/{colaboradorId}/revocar", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ValidarPerfilColaboradorAsync(Guid colaboradorId)
    {
        PrepararCliente();
        var response = await _http.PostAsync($"api/admin/colaboradores/{colaboradorId}/validar-perfil", null);
        return response.IsSuccessStatusCode;
    }
}
