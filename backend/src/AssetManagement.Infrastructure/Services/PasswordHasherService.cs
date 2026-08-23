using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace AssetManagement.Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<UserEntity> _passwordHasher;

        public PasswordHasherService()
        {
            _passwordHasher = new PasswordHasher<UserEntity>();
        }

        public string HashPassword(UserEntity user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(UserEntity user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
