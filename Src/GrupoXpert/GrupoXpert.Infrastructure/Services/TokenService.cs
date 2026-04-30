using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GrupoXpert.Application.Common.Interfaces;
using GrupoXpert.Domain.Identidad;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GrupoXpert.Infrastructure.Services;

/// <summary>
/// Implementación del servicio de generación de tokens JWT.
/// </summary>
public sealed class TokenService(IConfiguration configuracion) : ITokenService
{
    private readonly IConfiguration _configuracion = configuracion;

    public string GenerarToken(Usuario usuario)
    {
        var secreto = _configuracion["Jwt:Secreto"] ?? throw new InvalidOperationException("Configuración Jwt:Secreto no encontrada.");
        var emisor = _configuracion["Jwt:Emisor"] ?? "GrupoXpert";
        var audiencia = _configuracion["Jwt:Audiencia"] ?? "GrupoXpertUsers";
        var expiracionMinutos = int.Parse(_configuracion["Jwt:ExpiracionMinutos"] ?? "60");

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreto));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Email.Valor),
            new Claim("nombre", usuario.Nombre),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: emisor,
            audience: audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiracionMinutos),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

