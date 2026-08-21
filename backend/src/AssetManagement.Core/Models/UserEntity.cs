using System;

namespace AssetManagement.Core.Models
{
    public class UserEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Name
        {
            get
            {
                string combined = $"{FirstName} {LastName}".Trim();
                return string.IsNullOrWhiteSpace(combined) ? (Email?.Split('@')[0] ?? "User") : combined;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parts = value.Trim().Split(' ', 2);
                    FirstName = parts[0];
                    LastName = parts.Length > 1 ? parts[1] : string.Empty;
                }
            }
        }

        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "user"; // admin, manager, user
        public string Provider { get; set; } = "local"; // local, google, azure, github
        public string? AvatarUrl { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
