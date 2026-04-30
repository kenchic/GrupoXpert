using System.Net.Http.Json;
using System.Text.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Componentes.Identidad;

namespace GrupoXpert.Web.Services;

/// <summary>
/// Implementación del servicio de autenticación para la plataforma Web.
/// Realiza llamadas HTTP a la API de backend.
/// </summary>
public sealed class AutenticacionService(HttpClient http) : IAutenticacionService
{
    private readonly HttpClient _http = http;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public string ObtenerBaseUrl() => _http.BaseAddress?.ToString() ?? "N/A";

    public async Task RegistrarAsync(FormularioRegistro.ModeloRegistro modelo)
    {
        var response = await _http.PostAsJsonAsync("api/autenticacion/registrar", new 
        { 
            Email = modelo.Email,
            Clave = modelo.Clave,
            Nombre = modelo.Nombre,
            Imagen = (string?)null,
            ActivacionAutomatica = true
        });

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try 
            {
                var error = JsonSerializer.Deserialize<ApiError>(content, _jsonOptions);
                throw new Exception(error?.Mensaje ?? "Error al registrar el usuario.");
            }
            catch
            {
                throw new Exception($"Error del servidor ({response.StatusCode}): {content}");
            }
        }
    }

    public async Task<string> IniciarSesionAsync(string email, string clave)
    {
        var response = await _http.PostAsJsonAsync("api/autenticacion/iniciar-sesion", new 
        { 
            Email = email,
            Clave = clave
        });

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            try 
            {
                var error = JsonSerializer.Deserialize<ApiError>(content, _jsonOptions);
                throw new Exception(error?.Mensaje ?? "Credenciales inválidas.");
            }
            catch
            {
                throw new Exception("Error al intentar iniciar sesión.");
            }
        }

        try 
        {
            var result = JsonSerializer.Deserialize<LoginResponse>(content, _jsonOptions);
            return result?.Token ?? string.Empty;
        }
        catch
        {
            throw new Exception("La respuesta del servidor no tiene el formato esperado.");
        }
    }

    public async Task ActivarCuentaAsync(string token)
    {
        var response = await _http.GetAsync($"api/autenticacion/activar?token={token}");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ApiError>(content, _jsonOptions);
            throw new Exception(error?.Mensaje ?? "Error al activar la cuenta.");
        }
    }

    private record ApiError(string Mensaje);
    private record LoginResponse(string Token, string Email, string Nombre);
}
