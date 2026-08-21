using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, long expiresAt) GenerateToken(UserEntity user, bool rememberMe = true)
    {
        var secretKey = _configuration["JwtSettings:Secret"] 
            ?? "SuperSecretKeyForAssetManagementJwtSigning2026!#$";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var durationDays = rememberMe ? 30 : 1;
        var expiration = DateTime.UtcNow.AddDays(durationDays);
        var expiresAtTimestamp = new DateTimeOffset(expiration).ToUnixTimeMilliseconds();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role),
            new Claim("provider", user.Provider),
            new Claim("avatarUrl", user.AvatarUrl ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = _configuration["JwtSettings:Issuer"] ?? "AssetPulse.Api",
            Audience = _configuration["JwtSettings:Audience"] ?? "AssetPulse.Client",
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expiresAtTimestamp);
    }
}
