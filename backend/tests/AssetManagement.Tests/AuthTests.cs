using System.Threading.Tasks;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AssetManagement.Tests
{
    public class AuthTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            AppDbContext db = new AppDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task Login_ShouldReturnAuthResponse_WhenValidCredentialsProvided()
        {
            AppDbContext db = GetInMemoryDbContext();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            TokenService tokenService = new TokenService(config);
            PasswordHasherService hasher = new PasswordHasherService();
            TranslationService translationService = new TranslationService();
            AuthController controller = new AuthController(db, tokenService, hasher, translationService);

            LoginRequestDto request = new LoginRequestDto("admin@assetmgmt.io", "password123", RememberMe: true);
            IResult result = await controller.Login(request);

            Ok<AuthResponseDto> okResult = Assert.IsType<Ok<AuthResponseDto>>(result);
            AuthResponseDto? response = okResult.Value;

            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.Equal("admin@assetmgmt.io", response.User.Email);
            Assert.Equal("admin", response.User.Role);
        }

        [Fact]
        public async Task SsoLogin_ShouldReturnAuthResponse_ForGoogleProvider()
        {
            AppDbContext db = GetInMemoryDbContext();
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            TokenService tokenService = new TokenService(config);
            PasswordHasherService hasher = new PasswordHasherService();
            TranslationService translationService = new TranslationService();
            AuthController controller = new AuthController(db, tokenService, hasher, translationService);

            SsoLoginRequestDto request = new SsoLoginRequestDto("google", RememberMe: true);
            IResult result = await controller.SsoLogin(request);

            Ok<AuthResponseDto> okResult = Assert.IsType<Ok<AuthResponseDto>>(result);
            AuthResponseDto? response = okResult.Value;

            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.Equal("google", response.User.Provider);
            Assert.Contains("gmail.com", response.User.Email);
        }

        [Fact]
        public void TokenService_ShouldGenerateValidJwtToken()
        {
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            TokenService tokenService = new TokenService(config);

            UserEntity user = new UserEntity
            {
                Id = "d7c8a910-1234-4567-8901-abcdef123456",
                Username = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@assetmgmt.io",
                Role = "user",
                Provider = "local"
            };

            (string token, long expiresAt) = tokenService.GenerateToken(user, rememberMe: true);

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.True(expiresAt > System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }
    }
}
