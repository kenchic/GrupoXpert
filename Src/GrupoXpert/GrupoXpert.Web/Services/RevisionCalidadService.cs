using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Calidad;
using Microsoft.AspNetCore.Http;

namespace GrupoXpert.Web.Services;

public sealed class RevisionCalidadService(
    HttpClient http,
    IHttpContextAccessor httpContextAccessor)
    : IRevisionCalidadService
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

    public async Task<RevisionCalidadDto?> CrearRevisionAsync(Guid avanceId, Guid revisorId)
    {
        PrepararCliente();
        try
        {
            var comando = new { AvanceId = avanceId, RevisorId = revisorId };
            var response = await _http.PostAsJsonAsync("api/Calidad/revisiones", comando);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RevisionCalidadDto>();
            return null;
        }
        catch { return null; }
    }

    public async Task<RevisionCalidadDto?> OtorgarVistoBuenoAsync(Guid revisionId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsync($"api/Calidad/revisiones/{revisionId}/visto-bueno", null);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RevisionCalidadDto>();
            return null;
        }
        catch { return null; }
    }

    public async Task<RevisionCalidadDto?> RechazarRevisionAsync(Guid revisionId, string observaciones)
    {
        PrepararCliente();
        try
        {
            var comando = new { RevisionId = revisionId, Observaciones = observaciones };
            var response = await _http.PostAsJsonAsync($"api/Calidad/revisiones/{revisionId}/rechazar", comando);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RevisionCalidadDto>();
            return null;
        }
        catch { return null; }
    }

    public async Task<IReadOnlyList<RevisionCalidadDto>> ObtenerPendientesAsync()
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<RevisionCalidadDto>>("api/Calidad/revisiones/pendientes")
                   ?? new List<RevisionCalidadDto>();
        }
        catch { return new List<RevisionCalidadDto>(); }
    }

    public async Task<RevisionCalidadDto?> ObtenerPorAvanceAsync(Guid avanceId)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<RevisionCalidadDto>($"api/Calidad/revisiones/avance/{avanceId}");
        }
        catch { return null; }
    }
}
