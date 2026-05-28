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

    public async Task<IReadOnlyList<AvanceDto>> ObtenerAvancesAsync(Guid solicitudId)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<List<AvanceDto>>($"api/SolicitudesAcademicas/{solicitudId}/avances")
                   ?? new List<AvanceDto>();
        }
        catch
        {
            return new List<AvanceDto>();
        }
    }

    public async Task<AvanceDto?> SubirAvanceAsync(Guid solicitudId, Guid asesorId, string descripcion, int numeroFase, int tipo, IReadOnlyList<ArchivoSubidaModelo> archivos)
    {
        PrepararCliente();
        try
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(solicitudId.ToString()), "solicitudId");
            content.Add(new StringContent(asesorId.ToString()), "asesorId");
            content.Add(new StringContent(descripcion), "descripcion");
            content.Add(new StringContent(numeroFase.ToString()), "numeroFase");
            content.Add(new StringContent(tipo.ToString()), "tipo");

            foreach (var archivo in archivos)
            {
                var streamContent = new StreamContent(archivo.Contenido);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.TipoContenido);
                content.Add(streamContent, "archivos", archivo.NombreArchivo);
            }

            var response = await _http.PostAsync($"api/SolicitudesAcademicas/{solicitudId}/avances", content);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<AvanceDto>();
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> AgregarComentarioAsync(Guid avanceId, Guid autorId, string contenido)
    {
        PrepararCliente();
        try
        {
            var comando = new { AvanceId = avanceId, AutorId = autorId, Contenido = contenido };
            var response = await _http.PostAsJsonAsync($"api/SolicitudesAcademicas/avances/{avanceId}/comentarios", comando);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AgregarArchivoAsync(Guid avanceId, string nombreArchivo, string url, long tamanioBytes, string tipoContenido)
    {
        PrepararCliente();
        try
        {
            var comando = new { AvanceId = avanceId, NombreArchivo = nombreArchivo, Url = url, TamanioBytes = tamanioBytes, TipoContenido = tipoContenido };
            var response = await _http.PostAsJsonAsync($"api/SolicitudesAcademicas/avances/{avanceId}/archivos", comando);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AprobarAvanceAsync(Guid avanceId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsync($"api/SolicitudesAcademicas/avances/{avanceId}/aprobar", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RechazarAvanceAsync(Guid avanceId)
    {
        PrepararCliente();
        try
        {
            var response = await _http.PostAsync($"api/SolicitudesAcademicas/avances/{avanceId}/rechazar", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
