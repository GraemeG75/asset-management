using AssetManagement.Api.Controllers;
using AssetManagement.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace AssetManagement.Tests
{
    public class BootstrapTests
    {
        [Fact]
        public void GetUserBootstrap_ReturnsOkResultWithData()
        {
            FormMetadataController controller = new FormMetadataController();
            IResult result = controller.GetUserBootstrap();

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
