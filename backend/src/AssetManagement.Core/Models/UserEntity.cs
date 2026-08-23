using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Core.Models
{
    [Table("users")]
    public class UserEntity
    {
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string Name
        {
            get
            {
                string combined = $"{FirstName} {LastName}".Trim();
                return string.IsNullOrWhiteSpace(combined) ? (Username.Length > 0 ? Username : (Email?.Split('@')[0] ?? "User")) : combined;
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

        [Column("email")]
        public required string Email { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("role")]
        public string Role { get; set; } = "user"; // admin, manager, user

        [Column("provider")]
        public string Provider { get; set; } = "local"; // local, google, azure, github

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("preferred_language")]
        public string PreferredLanguage { get; set; } = "en-US";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
