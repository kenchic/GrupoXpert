using System.Text;
using GrupoXpert.Application;
using GrupoXpert.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de Capas (DDD)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Configuración de Autenticación JWT
var secreto = builder.Configuration["Jwt:Secreto"] ?? "GradoXpertSecretoSuperSeguro2026";
var emisor = builder.Configuration["Jwt:Emisor"] ?? "GradoXpert";
var audiencia = builder.Configuration["Jwt:Audiencia"] ?? "GradoXpertUsers";

builder.Services.AddAuthentication(opciones =>
{
    opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opciones =>
{
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = emisor,
        ValidAudience = audiencia,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreto))
    };
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. Pipeline de solicitudes
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Importante: Antes de Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

