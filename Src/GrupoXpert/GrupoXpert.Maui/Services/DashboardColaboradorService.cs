using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Perfil;
using Microsoft.Maui.Storage;

namespace GrupoXpert.Maui.Services;

public sealed class DashboardColaboradorService(HttpClient http) : IDashboardColaboradorService
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