using GrupoXpert.Shared.UI.Abstracciones;

namespace GrupoXpert.Maui.Services;

public sealed class DescargadorArchivos : IDescargadorArchivos
{
    public async Task DescargarAsync(string urlAbsoluta)
    {
        await Browser.Default.OpenAsync(urlAbsoluta, BrowserLaunchMode.SystemPreferred);
    }
}
