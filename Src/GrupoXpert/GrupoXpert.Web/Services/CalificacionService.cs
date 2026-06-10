using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Calificacion;
using Microsoft.AspNetCore.Http;

namespace GrupoXpert.Web.Services;

public sealed class CalificacionService(
    HttpClient http,
    IHttpContextAccessor httpContextAccessor)
    : ICalificacionService
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

    public async Task<CalificacionColaboradorDto?> CalificarAsync(Guid solicitudId, Guid clienteId, Guid colaboradorId, int puntaje, string? observacion)
    {
        PrepararCliente();
        try
        {
            var comando = new { SolicitudId = solicitudId, ClienteId = clienteId, ColaboradorId = colaboradorId, Puntaje = puntaje, Observacion = observacion };
            var response = await _http.PostAsJsonAsync("api/Calificacion", comando);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<CalificacionColaboradorDto>();
            return null;
        }
        catch { return null; }
    }

    public async Task<IReadOnlyList<SolicitudPorCalificarDto>> ObtenerSolicitudesPorCalificarAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<SolicitudPorCalificarDto>>("api/Calificacion/solicitudes-por-calificar")
                   ?? new List<SolicitudPorCalificarDto>();
        }
        catch { return new List<SolicitudPorCalificarDto>(); }
    }

    public async Task<CalificacionColaboradorDto?> ObtenerPorSolicitudAsync(Guid solicitudId)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<CalificacionColaboradorDto>($"api/Calificacion/solicitud/{solicitudId}");
        }
        catch { return null; }
    }

    public async Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerPorColaboradorAsync(Guid colaboradorId)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<CalificacionColaboradorDto>>($"api/Calificacion/colaborador/{colaboradorId}")
                   ?? new List<CalificacionColaboradorDto>();
        }
        catch { return new List<CalificacionColaboradorDto>(); }
    }

    public async Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerTodasAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<CalificacionColaboradorDto>>("api/Calificacion/todas")
                   ?? new List<CalificacionColaboradorDto>();
        }
        catch { return new List<CalificacionColaboradorDto>(); }
    }

    public async Task<IReadOnlyList<CalificacionColaboradorDto>> ObtenerMisCalificacionesAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<CalificacionColaboradorDto>>("api/Calificacion/mis-calificaciones")
                   ?? new List<CalificacionColaboradorDto>();
        }
        catch { return new List<CalificacionColaboradorDto>(); }
    }

    public async Task<bool> LiberarSolicitudAsync(Guid solicitudId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsync($"api/Calificacion/solicitud/{solicitudId}/liberar", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}