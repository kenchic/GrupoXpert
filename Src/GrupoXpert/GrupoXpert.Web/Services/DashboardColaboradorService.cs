using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Web.Services;

public sealed class DashboardColaboradorService(HttpClient http, IHttpContextAccessor httpContextAccessor) : IDashboardColaboradorService
{
    private readonly HttpClient _http = http;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private void PrepararCliente()
    {
        var token = _httpContextAccessor.HttpContext?.User?.FindFirst("jwt_token")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<DashboardColaboradorModelo?> ObtenerDashboardAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<DashboardColaboradorModelo>("api/dashboard/colaborador");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}