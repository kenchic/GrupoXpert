using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Academia;
using Microsoft.AspNetCore.Http;

namespace GrupoXpert.Web.Services;

public sealed class SolicitudAcademicaService(
    HttpClient http, 
    IHttpContextAccessor httpContextAccessor,
    IPerfilService perfilService) 
    : ISolicitudAcademicaService
{
    private readonly HttpClient _http = http;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPerfilService _perfilService = perfilService;

    private void PrepararCliente()
    {
        var token = _httpContextAccessor.HttpContext?.User?.FindFirst("jwt_token")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<bool> CrearSolicitudAsync(SolicitudAcademicaModelo modelo)
    {
        PrepararCliente();
        
        // Obtener el perfil del cliente para tener su ClienteId
        var perfil = await _perfilService.ObtenerPerfilActualAsync();
        if (perfil == null)
        {
            throw new InvalidOperationException("No se pudo obtener el perfil del cliente actual. Debes configurar tu perfil antes de hacer solicitudes.");
        }

        var comando = new
        {
            ClienteId = perfil.Id,
            NivelAcademico = modelo.NivelAcademico,
            TipoTrabajo = modelo.TipoTrabajo,
            AreaTematica = modelo.AreaTematica,
            FechaEntrega = modelo.FechaEntrega,
            NumeroPaginasOPalabras = modelo.NumeroPaginasOPalabras,
            NormaCitacion = modelo.NormaCitacion,
            Idioma = modelo.Idioma,
            FormatoRequerido = modelo.FormatoRequerido,
            MaterialBase = modelo.MaterialBase,
            EsUrgente = modelo.EsUrgente,
            EntregaPorFases = modelo.EntregaPorFases
        };

        var response = await _http.PostAsJsonAsync("api/SolicitudesAcademicas", comando);
        return response.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<SolicitudAcademicaDto>> ObtenerSolicitudesDashboardAsync()
    {
        PrepararCliente();
        
        try
        {
            var resultado = await _http.GetFromJsonAsync<List<SolicitudAcademicaDto>>("api/SolicitudesAcademicas/dashboard");
            return resultado ?? new List<SolicitudAcademicaDto>();
        }
        catch
        {
            return new List<SolicitudAcademicaDto>();
        }
    }

    public async Task<SolicitudAcademicaDto?> ObtenerPorIdAsync(Guid id)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<SolicitudAcademicaDto>($"api/SolicitudesAcademicas/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> AsignarAsesorAsync(Guid solicitudId, Guid asesorId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsJsonAsync($"api/SolicitudesAcademicas/{solicitudId}/asignar", asesorId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> PostularASolicitudAsync(Guid solicitudId, Guid colaboradorId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsJsonAsync($"api/SolicitudesAcademicas/{solicitudId}/postular", colaboradorId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SeleccionarPostuladoAsync(Guid solicitudId, Guid colaboradorId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsJsonAsync($"api/SolicitudesAcademicas/{solicitudId}/seleccionar-postulado", colaboradorId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
