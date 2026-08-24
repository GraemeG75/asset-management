using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AssetManagement.Tests
{
    public class AuditFieldsTests
    {
        private AppDbContext GetInMemoryDbContext(IUserContext userContext)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            AppDbContext db = new AppDbContext(options, userContext);
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public async Task AppDbContext_AutomaticallyPopulatesAuditFieldsOnCreateAndUpdate()
        {
            string actorUserId = Guid.NewGuid().ToString();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, actorUserId) }, "TestAuth")
            );
            DefaultHttpContext httpContext = new DefaultHttpContext { User = claimsPrincipal };
            HttpContextAccessor accessor = new HttpContextAccessor { HttpContext = httpContext };

            AppDbContext tempDb = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            UserContext userContext = new UserContext(accessor, tempDb);
            AppDbContext db = GetInMemoryDbContext(userContext);

            UserEntity newUser = new UserEntity
            {
                Username = "audituser",
                FirstName = "Audit",
                LastName = "User",
                Email = "audit@assetmgmt.io",
                Role = 4
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            Assert.NotEqual(default(DateTime), newUser.DateCreated);
            Assert.Equal(Guid.Parse(actorUserId), newUser.CreatedById);
            Assert.Null(newUser.DateUpdated);
            Assert.Null(newUser.UpdatedById);

            newUser.FirstName = "UpdatedAudit";
            await db.SaveChangesAsync();

            Assert.NotNull(newUser.DateUpdated);
            Assert.Equal(Guid.Parse(actorUserId), newUser.UpdatedById);
        }

        [Fact]
        public async Task MapperService_UpdatesAuditFieldsOnFormSubmission()
        {
            string actorUserId = Guid.NewGuid().ToString();
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, actorUserId) }, "TestAuth")
            );
            DefaultHttpContext httpContext = new DefaultHttpContext { User = claimsPrincipal };
            HttpContextAccessor accessor = new HttpContextAccessor { HttpContext = httpContext };

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            AppDbContext tempDb = new AppDbContext(options);
            UserContext userContext = new UserContext(accessor, tempDb);
            AppDbContext db = new AppDbContext(options, userContext);
            userContext = new UserContext(accessor, db);
            db.Database.EnsureCreated();

            UserEntity user = new UserEntity
            {
                Id = actorUserId,
                Username = "mapperuser",
                FirstName = "Mapper",
                LastName = "User",
                Email = "mapper@assetmgmt.io"
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            SiteContext siteContext = new SiteContext(accessor);
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();

            MapperService mapperService = new MapperService(siteContext, userContext, mockRepo.Object, db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "profile",
                FormKey = "profile_form",
                FormType = "standard",
                FieldValues = new System.Collections.Generic.Dictionary<string, object?>
                {
                    ["firstName"] = "NewMapperName"
                }
            };

            FormSubmissionResultDto result = await mapperService.SaveFormDataAsync(submission);

            Assert.True(result.Success);

            UserEntity? dbUser = await db.Users.FindAsync(actorUserId);
            Assert.NotNull(dbUser);
            Assert.NotNull(dbUser.DateUpdated);
            Assert.Equal(Guid.Parse(actorUserId), dbUser.UpdatedById);
        }
    }
}
