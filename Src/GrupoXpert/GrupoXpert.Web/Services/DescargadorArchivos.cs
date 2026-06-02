using GrupoXpert.Shared.UI.Abstracciones;
using Microsoft.JSInterop;

namespace GrupoXpert.Web.Services;

public sealed class DescargadorArchivos(IJSRuntime js) : IDescargadorArchivos
{
    public async Task DescargarAsync(string urlAbsoluta)
    {
        await js.InvokeVoidAsync("open", urlAbsoluta, "_blank");
    }
}
