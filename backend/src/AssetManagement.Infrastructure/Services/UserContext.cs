using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Services
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        public UserContext(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public ClaimsPrincipal? UserClaims => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => UserClaims?.Identity?.IsAuthenticated ?? false;

        public string? UserId
        {
            get
            {
                ClaimsPrincipal? user = UserClaims;
                if (user != null)
                {
                    string? id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        return id;
                    }
                }
                return null;
            }
        }

        public string? Username
        {
            get
            {
                ClaimsPrincipal? user = UserClaims;
                if (user != null)
                {
                    string? name = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("name");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        return name;
                    }
                }
                return null;
            }
        }

        public string? Email
        {
            get
            {
                ClaimsPrincipal? user = UserClaims;
                if (user != null)
                {
                    string? email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue("email");
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        return email;
                    }
                }
                return null;
            }
        }

        public int Role
        {
            get
            {
                ClaimsPrincipal? user = UserClaims;
                if (user != null)
                {
                    string? roleClaim = user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");
                    if (!string.IsNullOrWhiteSpace(roleClaim) && int.TryParse(roleClaim, out int roleId))
                    {
                        return roleId;
                    }
                }
                return 4; // Default to Standard User (4)
            }
        }

        public string PreferredLanguage
        {
            get
            {
                ClaimsPrincipal? user = UserClaims;
                if (user != null)
                {
                    string? lang = user.FindFirstValue("preferred_language") ?? user.FindFirstValue("lang");
                    if (!string.IsNullOrWhiteSpace(lang))
                    {
                        return lang;
                    }
                }
                return "en-US";
            }
        }

        public bool HasRole(int roleId)
        {
            return Role == roleId;
        }

        public bool IsAdmin()
        {
            return Role == 1;
        }

        public async Task<UserEntity?> GetCurrentUserEntityAsync()
        {
            string? id = UserId;
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
