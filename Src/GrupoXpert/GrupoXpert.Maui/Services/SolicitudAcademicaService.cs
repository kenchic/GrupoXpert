using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Academia;

namespace GrupoXpert.Maui.Services;

public sealed class SolicitudAcademicaService(HttpClient http) : ISolicitudAcademicaService
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

    public async Task<bool> CrearSolicitudAsync(SolicitudAcademicaModelo modelo)
    {
        PrepararCliente();
        
        // Obtener el perfil del cliente para tener su ClienteId
        // En MAUI, también podemos consultar la API para obtener el perfil actual
        var perfilResponse = await _http.GetAsync("api/perfil");
        if (!perfilResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("No se pudo obtener el perfil del cliente actual. Debes configurar tu perfil antes de hacer solicitudes.");
        }
        
        var perfil = await perfilResponse.Content.ReadFromJsonAsync<GrupoXpert.Shared.UI.Modelos.Perfil.PerfilClienteModelo>();
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
}
