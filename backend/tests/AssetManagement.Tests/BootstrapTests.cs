using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Api.Controllers;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace AssetManagement.Tests
{
    public class BootstrapTests
    {
        [Fact]
        public async Task GetUserBootstrap_ReturnsOkResultWithData()
        {
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();

            mockRepo.Setup(r => r.GetProfileNavLinksAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<XProfileNavLinkEntity>
                {
                    new XProfileNavLinkEntity
                    {
                        NavId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                        LinkKey = "profile-settings",
                        Label = "My Profile",
                        Icon = "user",
                        Url = "/profile",
                        DisplayOrder = 1
                    }
                });

            mockRepo.Setup(r => r.GetSiteNavLinksAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<XSiteNavLinkEntity>
                {
                    new XSiteNavLinkEntity
                    {
                        NavId = Guid.Parse("e8a719c2-570a-4a2e-9d2a-8d7d91e84321"),
                        LinkKey = "nav-dashboard",
                        Label = "Inbox & Dashboard",
                        Icon = "home",
                        Route = "/dashboard",
                        BadgeCount = 4,
                        Category = "Main",
                        DisplayOrder = 1
                    }
                });

            mockRepo.Setup(r => r.GetFormsForPageAsync("dashboard", It.IsAny<string>()))
                .ReturnsAsync(new List<XFormEntity>
                {
                    new XFormEntity
                    {
                        FormGuid = Guid.Parse("9a7b6c5d-4e3f-412a-8901-23456789abcd"),
                        FormKey = "asset-create",
                        FlavorKey = "flavor-asset-registration",
                        FormType = "standard",
                        Caption = "New Asset Registration Form",
                        Title = "Asset Registration",
                        SubmitButtonText = "Save Asset"
                    }
                });

            mockRepo.Setup(r => r.GetFlavorFieldsAsync("flavor-asset-registration", It.IsAny<string>()))
                .ReturnsAsync(new List<XMapperFlavorFieldEntity>
                {
                    new XMapperFlavorFieldEntity
                    {
                        FlavorFieldGuid = Guid.Parse("18f47638-4580-4c7a-a261-d8e7f6a51423"),
                        FlavorKey = "flavor-asset-registration",
                        KeyName = "assetTag",
                        Label = "Asset Tag Number",
                        FieldType = "text",
                        IsEditable = true
                    }
                });

            TranslationService translationService = new TranslationService();
            MetaController controller = new MetaController(mockRepo.Object, translationService);

            IResult result = await controller.GetUserBootstrap();

            Ok<UserBootstrapDto> okResult = Assert.IsType<Ok<UserBootstrapDto>>(result);
            UserBootstrapDto dto = okResult.Value!;

            Assert.NotNull(dto);
            Assert.NotEmpty(dto.ProfileNavLinks);
            Assert.NotEmpty(dto.SiteNavLinks);
            Assert.NotEmpty(dto.DashboardForms);
            Assert.True(dto.InboxCount > 0);
            Assert.Contains(dto.DashboardForms, f => f.FormType == "standard");
        }
    }
}
