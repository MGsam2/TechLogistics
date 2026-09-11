using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TechLogistics.Models;

namespace TechLogistics.Services;

public class JwtService
{
    private readonly IConfiguration configuration;

    public JwtService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public (string Token, DateTime Expira) GenerarToken(Usuario usuario)
    {
        var key = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "No está configurada la clave JWT.");
        }

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var expirationMinutes =
            configuration.GetValue<int>(
                "Jwt:ExpirationMinutes",
                120);

        var expira = DateTime.UtcNow.AddMinutes(
            expirationMinutes);

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new(
                ClaimTypes.Name,
                usuario.NombreUsuario),

            new(
                ClaimTypes.GivenName,
                usuario.NombreCompleto),

            new(
                ClaimTypes.Role,
                usuario.Rol!.Nombre)
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expira,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(
            tokenDescriptor);

        return (
            handler.WriteToken(token),
            expira);
    }
}