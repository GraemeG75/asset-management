using System.Threading.Tasks;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AssetManagement.Tests
{
    public class ProfileTests
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
        public async Task GetProfile_ShouldReturnCurrentAuthenticatedUserProfile()
        {
            AppDbContext db = GetInMemoryDbContext();
            ProfileController controller = new ProfileController(db);

            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            System.Security.Claims.ClaimsPrincipal claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
                }, "TestAuth")
            );

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            IResult result = await controller.GetProfile();

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? profile = okResult.Value;

            Assert.NotNull(profile);
            Assert.Equal(user.Email, profile.Email);
            Assert.Equal(user.PreferredLanguage, profile.PreferredLanguage);
        }

        [Fact]
        public async Task UpdateProfile_ShouldUpdateFirstNameLastNameLanguageAndAvatar()
        {
            AppDbContext db = GetInMemoryDbContext();
            ProfileController controller = new ProfileController(db);

            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            System.Security.Claims.ClaimsPrincipal claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
                }, "TestAuth")
            );

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            UpdateProfileDto request = new UpdateProfileDto("Alex", "Morgan", "de", "https://example.com/avatar.png");
            IResult result = await controller.UpdateProfile(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("Alex", updatedUser.FirstName);
            Assert.Equal("Morgan", updatedUser.LastName);
            Assert.Equal("Alex Morgan", updatedUser.Name);
            Assert.Equal("de", updatedUser.PreferredLanguage);

            UserEntity? dbUser = await db.Users.FindAsync(user.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("Alex", dbUser.FirstName);
            Assert.Equal("Morgan", dbUser.LastName);
            Assert.Equal("de", dbUser.PreferredLanguage);
        }

        [Fact]
        public async Task UpdateEmail_ShouldUpdateEmailWhenValidAndNotTaken()
        {
            AppDbContext db = GetInMemoryDbContext();
            ProfileController controller = new ProfileController(db);

            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            System.Security.Claims.ClaimsPrincipal claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
                }, "TestAuth")
            );

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            UpdateEmailDto request = new UpdateEmailDto("alex.newemail@assetmgmt.io");
            IResult result = await controller.UpdateEmail(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("alex.newemail@assetmgmt.io", updatedUser.Email);
        }

        [Fact]
        public async Task UpdateEmail_ShouldReturnBadRequestWhenEmailInvalid()
        {
            AppDbContext db = GetInMemoryDbContext();
            ProfileController controller = new ProfileController(db);

            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            System.Security.Claims.ClaimsPrincipal claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
                }, "TestAuth")
            );

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            UpdateEmailDto request = new UpdateEmailDto("not-an-email");
            IResult result = await controller.UpdateEmail(request);
            IStatusCodeHttpResult statusCodeResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task UpdateLanguage_ShouldUpdateUserPreferredLanguageInDb()
        {
            AppDbContext db = GetInMemoryDbContext();
            ProfileController controller = new ProfileController(db);

            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            System.Security.Claims.ClaimsPrincipal claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id)
                }, "TestAuth")
            );

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            UpdateLanguageRequestDto request = new UpdateLanguageRequestDto("fr");
            IResult result = await controller.UpdateLanguage(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("fr", updatedUser.PreferredLanguage);
        }
    }
}
