using AssetManagement.Core.Models;

namespace AssetManagement.Core.Services;

public interface ITokenService
{
    (string token, long expiresAt) GenerateToken(UserEntity user, bool rememberMe = true);
}
