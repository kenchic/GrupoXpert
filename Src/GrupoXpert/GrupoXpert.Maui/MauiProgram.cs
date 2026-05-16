using Microsoft.Extensions.Logging;
using Radzen;
namespace GrupoXpert.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddRadzenComponents();

            // Configuración de HttpClient para el Backend (Ajustar IP según entorno móvil)
            string baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5237/" : "http://localhost:5237/";
            
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri(baseAddress) 
            });

            builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IPerfilService, GrupoXpert.Maui.Services.PerfilService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
