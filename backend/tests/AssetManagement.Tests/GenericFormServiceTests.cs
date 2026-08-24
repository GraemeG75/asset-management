using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;
using AssetManagement.Infrastructure.Services;
using AssetManagement.Infrastructure.Services.FormHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AssetManagement.Tests
{
    public class GenericFormServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            AppDbContext db = new AppDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        private class MockMetadataRepository : IMetadataRepository
        {
            public Task<IEnumerable<XSiteNavLinkEntity>> GetSiteNavLinksAsync(string locale = "en-US") => Task.FromResult<IEnumerable<XSiteNavLinkEntity>>(new List<XSiteNavLinkEntity>());
            public Task<IEnumerable<XProfileNavLinkEntity>> GetProfileNavLinksAsync(string locale = "en-US") => Task.FromResult<IEnumerable<XProfileNavLinkEntity>>(new List<XProfileNavLinkEntity>());
            public Task<IEnumerable<XPageEntity>> GetPagesAsync(string locale = "en-US") => Task.FromResult<IEnumerable<XPageEntity>>(new List<XPageEntity>());
            public Task<XPageEntity?> GetPageByKeyAsync(string pageKey, string locale = "en-US") => Task.FromResult<XPageEntity?>(new XPageEntity { PageKey = pageKey, Title = "Test Page" });
            public Task<IEnumerable<XFormEntity>> GetFormsForPageAsync(string pageKey, string locale = "en-US") => Task.FromResult<IEnumerable<XFormEntity>>(new List<XFormEntity>());
            
            public Task<XFormEntity?> GetFormByKeyAsync(string formKey, string locale = "en-US")
            {
                return Task.FromResult<XFormEntity?>(new XFormEntity
                {
                    FormKey = formKey,
                    FormType = formKey.Contains("grid") ? "grid" : (formKey.Contains("search") ? "search" : (formKey.Contains("widget") ? "widget" : "standard")),
                    Title = "Test Form",
                    Caption = "Caption"
                });
            }

            public Task<IEnumerable<XMapperEntity>> GetMappersAsync(string locale = "en-US") => Task.FromResult<IEnumerable<XMapperEntity>>(new List<XMapperEntity>());
            public Task<IEnumerable<XMapperFlavorEntity>> GetMapperFlavorsAsync(string mapperKey, string locale = "en-US") => Task.FromResult<IEnumerable<XMapperFlavorEntity>>(new List<XMapperFlavorEntity>());
            public Task<IEnumerable<XMapperFlavorFieldEntity>> GetFlavorFieldsAsync(string flavorKey, string locale = "en-US") => Task.FromResult<IEnumerable<XMapperFlavorFieldEntity>>(new List<XMapperFlavorFieldEntity>());
        }

        private GenericFormService CreateService(AppDbContext db, IUserContext? userContext = null, ISiteContext? siteContext = null)
        {
            TranslationService translationService = new TranslationService();
            MockMetadataRepository repo = new MockMetadataRepository();
            MockSiteContext siteCtx = (siteContext as MockSiteContext) ?? new MockSiteContext();
            MockUserContext userCtx = (userContext as MockUserContext) ?? new MockUserContext(db) { UserId = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6" };

            MapperService mapperService = new MapperService(siteCtx, userCtx, repo, db);

            List<IFormTypeHandler> handlers = new List<IFormTypeHandler>
            {
                new StandardFormHandler(mapperService),
                new DetailFormHandler(),
                new GridFormHandler(),
                new SearchFormHandler(),
                new WidgetFormHandler()
            };

            FormHandlerFactory factory = new FormHandlerFactory(handlers);
            return new GenericFormService(repo, factory, translationService, siteCtx, userCtx);
        }

        [Fact]
        public async Task SubmitFormAsync_ShouldProcessStandardProfileForm_AndUpdateUserInDb()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "profile",
                FormKey = "profile-settings",
                FormType = "standard",
                FieldValues = new Dictionary<string, object?>
                {
                    ["firstName"] = "Jane",
                    ["lastName"] = "Doe",
                    ["preferredLanguage"] = "es-ES",
                    ["role"] = 2
                }
            };

            FormSubmissionResultDto result = await service.SubmitFormAsync(submission, "f81d4fae-7dec-11d0-a765-00a0c91e6bf6", "en-US");

            Assert.True(result.Success);
            Assert.Equal("profile-settings", result.FormKey);
            Assert.Equal("standard", result.FormType);

            UserEntity? updatedUser = await db.Users.FindAsync("f81d4fae-7dec-11d0-a765-00a0c91e6bf6");
            Assert.NotNull(updatedUser);
            Assert.Equal("Jane", updatedUser.FirstName);
            Assert.Equal("Doe", updatedUser.LastName);
            Assert.Equal("es-ES", updatedUser.PreferredLanguage);
            Assert.Equal(2, updatedUser.Role);
        }

        [Fact]
        public async Task SubmitFormAsync_ShouldProcessGridForm_ForCreateAction()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "assets",
                FormKey = "asset-grid-form",
                FormType = "grid",
                Action = "create",
                RecordId = "ast-101",
                FieldValues = new Dictionary<string, object?>
                {
                    ["assetName"] = "Industrial Laser Scanner",
                    ["serialNo"] = "SN-2026-99"
                }
            };

            FormSubmissionResultDto result = await service.SubmitFormAsync(submission, "user-123", "en-US");

            Assert.True(result.Success);
            Assert.Equal("ast-101", result.RecordId);
            Assert.Equal("grid", result.FormType);
            Assert.Contains("created successfully", result.Message);
        }

        [Fact]
        public async Task SubmitFormAsync_ShouldProcessSearchForm_AndReturnActiveFilters()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "audits",
                FormKey = "audit-search-form",
                FormType = "search",
                Action = "search",
                FieldValues = new Dictionary<string, object?>
                {
                    ["query"] = "Compliance Audit 2026",
                    ["status"] = "Pending",
                    ["emptyField"] = ""
                }
            };

            FormSubmissionResultDto result = await service.SubmitFormAsync(submission, "user-123", "en-US");

            Assert.True(result.Success);
            Assert.Equal("search", result.FormType);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data["appliedCount"]);
        }

        [Fact]
        public async Task SubmitFormAsync_ShouldProcessWidgetForm_AndReturnExecutionState()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "dashboard",
                FormKey = "kpi-widget-form",
                FormType = "widget",
                Action = "refresh",
                FieldValues = new Dictionary<string, object?>
                {
                    ["metricKey"] = "total_active_assets"
                }
            };

            FormSubmissionResultDto result = await service.SubmitFormAsync(submission, "user-123", "en-US");

            Assert.True(result.Success);
            Assert.Equal("widget", result.FormType);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.ContainsKey("executedAt"));
        }

        private class MockSiteContext : ISiteContext
        {
            public string CurrentLocale => "en-US";
            public string HttpMethod => "POST";
            public string RequestPath => "/api/form-data/submit";
            public string Host => "localhost";
            public string ClientIpAddress => "127.0.0.1";
            public string? UserAgent => "TestAgent";
            public DateTime RequestTimestamp => DateTime.UtcNow;
            public IDictionary<string, object?> Items => new Dictionary<string, object?>();
        }

        private class MockUserContext : IUserContext
        {
            private readonly AppDbContext? _db;

            public MockUserContext(AppDbContext? db = null)
            {
                _db = db;
            }

            public string? UserId { get; set; }
            public string? Username { get; set; } = "testuser";
            public string? Email { get; set; } = "test@assetmgmt.io";
            public int Role { get; set; } = 4;
            public string PreferredLanguage => "en-US";
            public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);
            public System.Security.Claims.ClaimsPrincipal? UserClaims => null;

            public bool HasRole(int roleId)
            {
                return Role == roleId;
            }

            public bool IsAdmin()
            {
                return Role == 1;
            }

            public async Task<UserEntity?> GetCurrentUserEntityAsync()
            {
                if (string.IsNullOrWhiteSpace(UserId) || _db == null)
                {
                    return null;
                }
                return await _db.Users.FindAsync(UserId);
            }
        }

        [Fact]
        public async Task Controller_SubmitForm_ShouldThrowUnauthorizedAccessException_WhenUserClaimMissing()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);
            MockUserContext unauthContext = new MockUserContext { UserId = null };
            MockSiteContext siteContext = new MockSiteContext();

            AssetManagement.Api.Controllers.FormSubmissionController controller = new AssetManagement.Api.Controllers.FormSubmissionController(service, unauthContext, siteContext);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "profile",
                FormKey = "profile-settings"
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.SubmitForm(submission));
        }

        [Fact]
        public async Task Controller_SubmitForm_ShouldSucceed_WhenUserIsAuthenticated()
        {
            AppDbContext db = GetInMemoryDbContext();
            GenericFormService service = CreateService(db);
            MockUserContext authContext = new MockUserContext { UserId = "user-123", Role = 1 };
            MockSiteContext siteContext = new MockSiteContext();

            AssetManagement.Api.Controllers.FormSubmissionController controller = new AssetManagement.Api.Controllers.FormSubmissionController(service, authContext, siteContext);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "profile",
                FormKey = "profile-settings",
                FieldValues = new Dictionary<string, object?> { ["theme"] = "dark" }
            };

            Microsoft.AspNetCore.Http.IResult result = await controller.SubmitForm(submission);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task MapperService_SaveFormDataAsync_ShouldUpdateDatabase_UsingSiteContextAndUserContext()
        {
            AppDbContext db = GetInMemoryDbContext();
            MockSiteContext siteContext = new MockSiteContext();
            MockUserContext userContext = new MockUserContext(db) { UserId = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6" };
            MockMetadataRepository metadataRepo = new MockMetadataRepository();

            MapperService mapperService = new MapperService(siteContext, userContext, metadataRepo, db);

            FormSubmissionDto submission = new FormSubmissionDto
            {
                PageKey = "profile",
                FormKey = "profile-settings",
                FormType = "standard",
                FieldValues = new Dictionary<string, object?>
                {
                    ["firstName"] = "Alex",
                    ["lastName"] = "Smith",
                    ["role"] = 3
                }
            };

            FormSubmissionResultDto result = await mapperService.SaveFormDataAsync(submission);

            Assert.True(result.Success);
            Assert.Equal("f81d4fae-7dec-11d0-a765-00a0c91e6bf6", result.RecordId);

            UserEntity? user = await db.Users.FindAsync("f81d4fae-7dec-11d0-a765-00a0c91e6bf6");
            Assert.NotNull(user);
            Assert.Equal("Alex", user.FirstName);
            Assert.Equal("Smith", user.LastName);
            Assert.Equal(3, user.Role);
        }

        [Fact]
        public async Task MapperService_LoadFormDataAsync_ShouldReturnPopulatedDictionary()
        {
            AppDbContext db = GetInMemoryDbContext();
            MockSiteContext siteContext = new MockSiteContext();
            MockUserContext userContext = new MockUserContext(db) { UserId = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6" };
            MockMetadataRepository metadataRepo = new MockMetadataRepository();

            MapperService mapperService = new MapperService(siteContext, userContext, metadataRepo, db);

            Dictionary<string, object?> loadedData = await mapperService.LoadFormDataAsync("profile-settings");

            Assert.NotNull(loadedData);
            Assert.True(loadedData.ContainsKey("Email") || loadedData.ContainsKey("email"));
        }
    }
}
