using System.Net.Http.Headers;
using System.Net.Http.Json;
using GrupoXpert.Shared.UI.Abstracciones;
using GrupoXpert.Shared.UI.Modelos.Perfil;

namespace GrupoXpert.Web.Services;

public sealed class PerfilColaboradorService(HttpClient http, IHttpContextAccessor httpContextAccessor) : IPerfilColaboradorService
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

    public async Task<PerfilColaboradorModelo?> ObtenerPerfilColaboradorAsync(Guid usuarioId)
    {
        PrepararCliente();
        try
        {
            return await _http.GetFromJsonAsync<PerfilColaboradorModelo>($"api/PerfilColaborador/{usuarioId}");
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

    public async Task<bool> ActualizarPerfilColaboradorAsync(PerfilColaboradorModelo modelo)
    {
        PrepararCliente();
        var respuesta = await _http.PutAsJsonAsync("api/PerfilColaborador", new
        {
            modelo.UsuarioId,
            modelo.NivelAcademico,
            modelo.DisponibilidadHorasSemana,
            modelo.CargaAcademicaIdeal,
            modelo.AreasConocimiento,
            modelo.TiposTrabajo,
            modelo.Idiomas,
            modelo.NormasCitacion
        });
        return respuesta.IsSuccessStatusCode;
    }
}
