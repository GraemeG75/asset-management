using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string token, long expiresAt) GenerateToken(UserEntity user, bool rememberMe = true)
        {
            string secretKey = _configuration["JwtSettings:Secret"] 
                ?? "SuperSecretKeyForAssetManagementJwtSigning2026!#$";
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int durationDays = rememberMe ? 30 : 1;
            DateTime expiration = DateTime.UtcNow.AddDays(durationDays);
            long expiresAtTimestamp = new DateTimeOffset(expiration).ToUnixTimeMilliseconds();

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role.ToString()),
                new Claim("provider", user.Provider),
                new Claim("avatarUrl", user.AvatarUrl ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _configuration["JwtSettings:Issuer"] ?? "AssetPulse.Api",
                Audience = _configuration["JwtSettings:Audience"] ?? "AssetPulse.Client",
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), expiresAtTimestamp);
        }
    }
}
