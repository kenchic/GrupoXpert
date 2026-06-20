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
            builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.ISolicitudAcademicaService, GrupoXpert.Maui.Services.SolicitudAcademicaService>();
            builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IAdminService, GrupoXpert.Maui.Services.AdminService>();
            builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IPerfilColaboradorService, GrupoXpert.Maui.Services.PerfilColaboradorService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IRevisionCalidadService, GrupoXpert.Maui.Services.RevisionCalidadService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.ICalificacionService, GrupoXpert.Maui.Services.CalificacionService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IDescargadorArchivos, GrupoXpert.Maui.Services.DescargadorArchivos>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IDashboardColaboradorService, GrupoXpert.Maui.Services.DashboardColaboradorService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
