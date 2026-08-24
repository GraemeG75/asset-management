using System.Security.Claims;
using System.Threading.Tasks;
using AssetManagement.Core.Models;

namespace AssetManagement.Core.Services
{
    public interface IUserContext
    {
        string? UserId { get; }
        string? Username { get; }
        string? Email { get; }
        int Role { get; }
        string PreferredLanguage { get; }
        bool IsAuthenticated { get; }
        ClaimsPrincipal? UserClaims { get; }

        bool HasRole(int roleId);
        bool IsAdmin();
        Task<UserEntity?> GetCurrentUserEntityAsync();
    }
}
