using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
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

        private ProfileController CreateController(AppDbContext db, UserEntity user)
        {
            TranslationService translationService = new TranslationService();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id) }, "TestAuth")
            );
            DefaultHttpContext httpContext = new DefaultHttpContext { User = claimsPrincipal };
            HttpContextAccessor accessor = new HttpContextAccessor { HttpContext = httpContext };

            UserContext userContext = new UserContext(accessor, db);
            SiteContext siteContext = new SiteContext(accessor);

            ProfileController controller = new ProfileController(db, translationService, userContext, siteContext)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = httpContext }
            };
            return controller;
        }

        [Fact]
        public async Task GetProfile_ShouldReturnCurrentAuthenticatedUserProfile()
        {
            AppDbContext db = GetInMemoryDbContext();
            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            ProfileController controller = CreateController(db, user);

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
            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            ProfileController controller = CreateController(db, user);

            Dictionary<string, object?> request = new Dictionary<string, object?>
            {
                ["firstName"] = "Alex",
                ["lastName"] = "Morgan",
                ["preferredLanguage"] = "de-DE",
                ["avatarUrl"] = "https://example.com/avatar.png"
            };

            IResult result = await controller.UpdateProfile(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("Alex", updatedUser.FirstName);
            Assert.Equal("Morgan", updatedUser.LastName);
            Assert.Equal("Alex Morgan", updatedUser.Name);
            Assert.Equal("de-de", updatedUser.PreferredLanguage);
        }

        [Fact]
        public async Task UpdateEmail_ShouldUpdateEmailWhenValidAndNotTaken()
        {
            AppDbContext db = GetInMemoryDbContext();
            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            ProfileController controller = CreateController(db, user);

            Dictionary<string, string> request = new Dictionary<string, string>
            {
                ["newEmail"] = "alex.newemail@assetmgmt.io"
            };

            IResult result = await controller.UpdateEmail(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("alex.newemail@assetmgmt.io", updatedUser.Email);
        }

        [Fact]
        public async Task UpdateEmail_ShouldReturnLocalizedBadRequestWhenEmailInvalid()
        {
            AppDbContext db = GetInMemoryDbContext();
            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);
            user.PreferredLanguage = "es-ES";
            await db.SaveChangesAsync();

            ProfileController controller = CreateController(db, user);

            Dictionary<string, string> request = new Dictionary<string, string>
            {
                ["newEmail"] = "not-an-email"
            };

            IResult result = await controller.UpdateEmail(request);
            IStatusCodeHttpResult statusCodeResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task UpdateLanguage_ShouldUpdateUserPreferredLanguageInDb()
        {
            AppDbContext db = GetInMemoryDbContext();
            UserEntity? user = await db.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);

            ProfileController controller = CreateController(db, user);

            Dictionary<string, string> request = new Dictionary<string, string>
            {
                ["language"] = "fr-FR"
            };

            IResult result = await controller.UpdateLanguage(request);

            Ok<UserDto> okResult = Assert.IsType<Ok<UserDto>>(result);
            UserDto? updatedUser = okResult.Value;

            Assert.NotNull(updatedUser);
            Assert.Equal("fr-fr", updatedUser.PreferredLanguage);
        }
    }
}
