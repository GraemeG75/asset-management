using System.Threading.Tasks;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AssetManagement.Tests;

public class AuthTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public async Task Login_ShouldReturnAuthResponse_WhenValidCredentialsProvided()
    {
        var db = GetInMemoryDbContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var tokenService = new TokenService(config);
        var controller = new AuthController(db, tokenService);

        var request = new LoginRequestDto("admin@assetmgmt.io", "password123", RememberMe: true);
        var actionResult = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.NotNull(response.Token);
        Assert.Equal("admin@assetmgmt.io", response.User.Email);
        Assert.Equal("admin", response.User.Role);
    }

    [Fact]
    public async Task SsoLogin_ShouldReturnAuthResponse_ForGoogleProvider()
    {
        var db = GetInMemoryDbContext();
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var tokenService = new TokenService(config);
        var controller = new AuthController(db, tokenService);

        var request = new SsoLoginRequestDto("google", RememberMe: true);
        var actionResult = await controller.SsoLogin(request);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.NotNull(response.Token);
        Assert.Equal("google", response.User.Provider);
        Assert.Contains("gmail.com", response.User.Email);
    }

    [Fact]
    public void TokenService_ShouldGenerateValidJwtToken()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var tokenService = new TokenService(config);

        var user = new UserEntity
        {
            Id = "usr_test_123",
            Name = "Test User",
            Email = "test@assetmgmt.io",
            Role = "user",
            Provider = "local"
        };

        var (token, expiresAt) = tokenService.GenerateToken(user, rememberMe: true);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.True(expiresAt > System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }
}
