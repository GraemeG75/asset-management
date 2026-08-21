using AssetManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity => {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Seed initial demo users into the database
        modelBuilder.Entity<UserEntity>().HasData(
            new UserEntity
            {
                Id = "usr_admin_demo",
                Name = "Admin User",
                Email = "admin@assetmgmt.io",
                PasswordHash = "password123",
                Role = "admin",
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=admin%40assetmgmt.io",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserEntity
            {
                Id = "usr_manager_demo",
                Name = "Manager User",
                Email = "manager@assetmgmt.io",
                PasswordHash = "password123",
                Role = "manager",
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=manager%40assetmgmt.io",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new UserEntity
            {
                Id = "usr_standard_demo",
                Name = "Standard User",
                Email = "user@assetmgmt.io",
                PasswordHash = "password123",
                Role = "user",
                Provider = "local",
                AvatarUrl = "https://api.dicebear.com/7.x/bottts/svg?seed=user%40assetmgmt.io",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
