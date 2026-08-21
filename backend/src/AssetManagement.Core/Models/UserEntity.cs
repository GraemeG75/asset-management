using System;

namespace AssetManagement.Core.Models
{
    public class UserEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "user"; // admin, manager, user
        public string Provider { get; set; } = "local"; // local, google, azure, github
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

