using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using Moq;
using Xunit;

namespace AssetManagement.Tests
{
    public class DapperMetadataTests
    {
        [Fact]
        public async Task GetSiteNavLinksAsync_ReturnsSiteNavLinksWithLocalization()
        {
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();
            mockRepo.Setup(r => r.GetSiteNavLinksAsync("en-US"))
                .ReturnsAsync(new List<XSiteNavLinkEntity>
                {
                    new XSiteNavLinkEntity
                    {
                        NavId = Guid.Parse("e8a719c2-570a-4a2e-9d2a-8d7d91e84321"),
                        LinkKey = "nav-dashboard",
                        Label = "Inbox & Dashboard",
                        Icon = "home",
                        Route = "/dashboard"
                    }
                });

            mockRepo.Setup(r => r.GetSiteNavLinksAsync("es-ES"))
                .ReturnsAsync(new List<XSiteNavLinkEntity>
                {
                    new XSiteNavLinkEntity
                    {
                        NavId = Guid.Parse("e8a719c2-570a-4a2e-9d2a-8d7d91e84321"),
                        LinkKey = "nav-dashboard",
                        Label = "Bandeja de Entrada",
                        Icon = "home",
                        Route = "/dashboard"
                    }
                });

            IEnumerable<XSiteNavLinkEntity> enLinks = await mockRepo.Object.GetSiteNavLinksAsync("en-US");
            IEnumerable<XSiteNavLinkEntity> esLinks = await mockRepo.Object.GetSiteNavLinksAsync("es-ES");

            Assert.NotEmpty(enLinks);
            Assert.NotEmpty(esLinks);

            XSiteNavLinkEntity enLink = Assert.Single(enLinks);
            XSiteNavLinkEntity esLink = Assert.Single(esLinks);

            Assert.Equal("Inbox & Dashboard", enLink.Label);
            Assert.Equal("Bandeja de Entrada", esLink.Label);
        }

        [Fact]
        public async Task GetPageByKeyAsync_ReturnsPageMetadata()
        {
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();
            mockRepo.Setup(r => r.GetPageByKeyAsync("dashboard", "en-US"))
                .ReturnsAsync(new XPageEntity
                {
                    PageGuid = Guid.Parse("d9b2e8f1-4c7a-412e-8901-b2c3d4e5f607"),
                    PageKey = "dashboard",
                    Title = "Operational Dashboard",
                    Description = "Main Workspace"
                });

            XPageEntity? page = await mockRepo.Object.GetPageByKeyAsync("dashboard", "en-US");

            Assert.NotNull(page);
            Assert.Equal("dashboard", page.PageKey);
            Assert.Equal("Operational Dashboard", page.Title);
        }

        [Fact]
        public async Task GetFormsForPageAsync_ReturnsFormAndVisibleClause()
        {
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();
            mockRepo.Setup(r => r.GetFormsForPageAsync("dashboard", "en-US"))
                .ReturnsAsync(new List<XFormEntity>
                {
                    new XFormEntity
                    {
                        FormGuid = Guid.Parse("9a7b6c5d-4e3f-412a-8901-23456789abcd"),
                        FormKey = "asset-create",
                        Caption = "New Asset Registration Form",
                        VisibleClause = "user.isAuthenticated"
                    }
                });

            IEnumerable<XFormEntity> forms = await mockRepo.Object.GetFormsForPageAsync("dashboard", "en-US");

            Assert.NotEmpty(forms);
            XFormEntity form = Assert.Single(forms);

            Assert.Equal("asset-create", form.FormKey);
            Assert.Equal("New Asset Registration Form", form.Caption);
            Assert.Equal("user.isAuthenticated", form.VisibleClause);
        }

        [Fact]
        public async Task GetFlavorFieldsAsync_ReturnsFlavorFields()
        {
            Mock<IMetadataRepository> mockRepo = new Mock<IMetadataRepository>();
            mockRepo.Setup(r => r.GetFlavorFieldsAsync("flavor-asset-registration", "en-US"))
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

            IEnumerable<XMapperFlavorFieldEntity> fields = await mockRepo.Object.GetFlavorFieldsAsync("flavor-asset-registration", "en-US");

            Assert.NotEmpty(fields);
            XMapperFlavorFieldEntity field = Assert.Single(fields);

            Assert.Equal("assetTag", field.KeyName);
            Assert.Equal("Asset Tag Number", field.Label);
            Assert.Equal("text", field.FieldType);
            Assert.True(field.IsEditable);
        }
    }
}
