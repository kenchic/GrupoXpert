using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GrupoXpert.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();

// Configurar HttpClient para el servidor Blazor (llamadas server-to-api)
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5237/") 
});

builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IAutenticacionService, GrupoXpert.Web.Services.AutenticacionService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IAdminService, GrupoXpert.Web.Services.AdminService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IPerfilService, GrupoXpert.Web.Services.PerfilService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IPerfilColaboradorService, GrupoXpert.Web.Services.PerfilColaboradorService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.ISolicitudAcademicaService, GrupoXpert.Web.Services.SolicitudAcademicaService>();
builder.Services.AddScoped<GrupoXpert.Shared.UI.Abstracciones.IDescargadorArchivos, GrupoXpert.Web.Services.DescargadorArchivos>();
builder.Services.AddHttpContextAccessor();

// Configurar servicios de autenticación y autorización basada en Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Endpoint para establecer la cookie de autenticación desde el JWT
// Se llama desde el navegador vía form POST o JS fetch
app.MapPost("/api/auth/cookie-login", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var jwt = form["jwt"].ToString();

    if (string.IsNullOrEmpty(jwt))
        return Results.Redirect("/login");

    try
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var claims = new List<Claim>();
        
        foreach (var claim in token.Claims)
        {
            claims.Add(claim);
        }
        
        // Asegurar que el claim de Role esté mapeado para AuthorizeView
        var roleClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role 
            || c.Type == "role" 
            || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        if (roleClaim != null && !claims.Any(c => c.Type == ClaimTypes.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
        }

        // Guardar el JWT completo como claim para reenviarlo a la API
        claims.Add(new Claim("jwt_token", jwt));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await ctx.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        // Redirigir al home — el navegador recibirá la cookie Set-Cookie
        return Results.Redirect("/");
    }
    catch
    {
        return Results.Redirect("/login");
    }
}).AllowAnonymous().DisableAntiforgery();

// Endpoint para cerrar sesión (limpiar cookie)
app.MapGet("/api/auth/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
