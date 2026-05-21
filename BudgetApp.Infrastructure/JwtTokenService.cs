using BudgetApp.Application.Interfaces;
using BudgetApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BudgetApp.Infrastructure;

// Implementerar IJwtTokenService — skapar JWT-tokens för inloggade användare
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    // IConfiguration injiceras för att läsa JWT-inställningar från appsettings.json
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Skapar en JWT-token för en given användare
    public string GenerateToken(User user)
    {
        // Hämtar JWT-inställningar från appsettings.json
        var secret = _configuration["Jwt:Secret"]!;
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

        // Skapar claims — data om användaren som lagras i token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // Skapar signeringsnyckeln från hemlig nyckel
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Skapar JWT-token med claims, utgångstid och signaturnyckel
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        // Returnerar token som sträng
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}