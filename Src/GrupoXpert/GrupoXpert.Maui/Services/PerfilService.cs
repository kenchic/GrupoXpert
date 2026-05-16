using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Maui.Services;

public sealed class PerfilService(HttpClient http) : IPerfilService
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

    public async Task<PerfilClienteModelo?> ObtenerPerfilActualAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<PerfilClienteModelo>("api/perfil");
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

    public async Task<bool> ActualizarPerfilAsync(PerfilClienteModelo modelo)
    {
        PrepararCliente();
        var response = await _http.PostAsJsonAsync("api/perfil", modelo);
        return response.IsSuccessStatusCode;
    }
}
