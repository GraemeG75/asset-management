using System;
using System.ComponentModel.DataAnnotations.Schema;
using AssetManagement.Core.Generated.PickLists;

namespace AssetManagement.Core.Models
{
    [Table("users")]
    public class UserEntity : IAuditEntity
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
                if (string.IsNullOrWhiteSpace(combined))
                {
                    return Username.Length > 0 ? Username : (Email?.Split('@')[0] ?? "User");
                }
                return combined;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] parts = value.Trim().Split(' ', 2);
                    FirstName = parts[0];
                    if (parts.Length > 1)
                    {
                        LastName = parts[1];
                    }
                    else
                    {
                        LastName = string.Empty;
                    }
                }
            }
        }

        [Column("email")]
        public required string Email { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("role")]
        public int Role { get; set; } = (int)UserRolesEnum.StandardUser;

        [NotMapped]
        public UserRolesEnum RoleEnum => (UserRolesEnum)Role;

        [NotMapped]
        public string RoleName => UserRolesPickList.GetName(Role);

        [Column("provider")]
        public string Provider { get; set; } = "local";

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("preferred_language")]
        public string PreferredLanguage { get; set; } = "en-US";

        [Column("date_created")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Column("created_by_id")]
        public Guid? CreatedById { get; set; }

        [Column("date_updated")]
        public DateTime? DateUpdated { get; set; }

        [Column("updated_by_id")]
        public Guid? UpdatedById { get; set; }
    }
}
