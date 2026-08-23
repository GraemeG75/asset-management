using System.Data;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Xunit;

namespace AssetManagement.Tests
{
    public class BootstrapTests
    {
        [Fact]
        public async Task GetUserBootstrap_ReturnsOkResultWithData()
        {
            using SqliteConnection connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            SqliteConnectionFactory factory = new SqliteConnectionFactory(connection);
            DapperMetadataRepository repository = new DapperMetadataRepository(factory);
            MetaController controller = new MetaController(repository);

            IResult result = await controller.GetUserBootstrap();

            Ok<UserBootstrapDto> okResult = Assert.IsType<Ok<UserBootstrapDto>>(result);
            UserBootstrapDto dto = okResult.Value!;

            Assert.NotNull(dto);
            Assert.NotEmpty(dto.ProfileNavLinks);
            Assert.NotEmpty(dto.SiteNavLinks);
            Assert.NotEmpty(dto.DashboardForms);
            Assert.True(dto.InboxCount > 0);

            // Verify form types present in bootstrap
            Assert.Contains(dto.DashboardForms, f => f.FormType == "widget");
            Assert.Contains(dto.DashboardForms, f => f.FormType == "search");
            Assert.Contains(dto.DashboardForms, f => f.FormType == "grid");
            Assert.Contains(dto.DashboardForms, f => f.FormType == "detail");
        }
    }
}
