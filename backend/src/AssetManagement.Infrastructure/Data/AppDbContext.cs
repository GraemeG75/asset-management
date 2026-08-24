using System;
using AssetManagement.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users => Set<UserEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(64).IsRequired();
                entity.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(64).IsRequired();
                entity.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(64).IsRequired();
                entity.Ignore(u => u.Name);
                entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
                entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(512);
                entity.Property(u => u.Role).HasColumnName("role").HasDefaultValue(4);
                entity.Property(u => u.Provider).HasColumnName("provider").HasMaxLength(32).HasDefaultValue("local");
                entity.Property(u => u.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(512);
                entity.Property(u => u.PreferredLanguage).HasColumnName("preferred_language").HasMaxLength(10).HasDefaultValue("en-US");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            PasswordHasher<UserEntity> hasher = new PasswordHasher<UserEntity>();

            UserEntity admin = new UserEntity
            {
                Id = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6",
                Username = "admin",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@assetmgmt.io",
                Role = 1,
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=admin%40assetmgmt.io",
                PreferredLanguage = "en-US",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            admin.PasswordHash = hasher.HashPassword(admin, "password123");

            UserEntity manager = new UserEntity
            {
                Id = "a1b2c3d4-e5f6-4789-8012-3456789abcde",
                Username = "manager",
                FirstName = "Manager",
                LastName = "User",
                Email = "manager@assetmgmt.io",
                Role = 2,
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=manager%40assetmgmt.io",
                PreferredLanguage = "en-US",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            manager.PasswordHash = hasher.HashPassword(manager, "password123");

            UserEntity standard = new UserEntity
            {
                Id = "b2c3d4e5-f6a7-4890-9123-456789abcdef",
                Username = "user",
                FirstName = "Standard",
                LastName = "User",
                Email = "user@assetmgmt.io",
                Role = 4,
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=user%40assetmgmt.io",
                PreferredLanguage = "en-US",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            standard.PasswordHash = hasher.HashPassword(standard, "password123");

            modelBuilder.Entity<UserEntity>().HasData(admin, manager, standard);
        }
    }
}
