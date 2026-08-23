using AssetManagement.Core.Models;

namespace AssetManagement.Core.Services
{
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Hashes a plain-text password using a cryptographically secure one-way salted hash (PBKDF2 HMAC-SHA512).
        /// </summary>
        string HashPassword(UserEntity user, string password);

        /// <summary>
        /// Verifies a provided plain-text password against a stored one-way salted password hash.
        /// </summary>
        bool VerifyPassword(UserEntity user, string hashedPassword, string providedPassword);
    }
}
