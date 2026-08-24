using System;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Generated.PickLists;
using AssetManagement.Core.Models;
using Xunit;

namespace AssetManagement.Tests
{
    public class PickListSourceGeneratorTests
    {
        [Fact]
        public void UserRolesPickList_GeneratedSuccessfully_WithValuesAndNames()
        {
            Assert.Equal("user_roles", UserRolesPickList.PickListName);
            Assert.Equal(1, UserRolesPickList.Values.Administrator);
            Assert.Equal(2, UserRolesPickList.Values.AssetManager);
            Assert.Equal(3, UserRolesPickList.Values.ComplianceOfficer);
            Assert.Equal(4, UserRolesPickList.Values.StandardUser);
            Assert.Equal(5, UserRolesPickList.Values.ReadOnly);

            Assert.Equal("Administrator", UserRolesPickList.Names.Administrator);
            Assert.Equal("Asset Manager", UserRolesPickList.Names.AssetManager);
            Assert.Equal("Compliance Officer", UserRolesPickList.Names.ComplianceOfficer);
            Assert.Equal("Standard User", UserRolesPickList.Names.StandardUser);
            Assert.Equal("Read Only", UserRolesPickList.Names.ReadOnly);

            Assert.Equal("Administrator", UserRolesPickList.GetName(1));
            Assert.Equal("Asset Manager", UserRolesPickList.GetName(2));
            Assert.Equal("Read Only", UserRolesPickList.GetName(5));
            Assert.Equal("Unknown", UserRolesPickList.GetName(999));

            Assert.Equal("Administrator", UserRolesPickList.GetName(UserRolesEnum.Administrator));
            Assert.Equal("Asset Manager", UserRolesPickList.GetName(UserRolesEnum.AssetManager));
        }

        [Fact]
        public void AssetStatusPickList_GeneratedSuccessfully_WithValuesAndNames()
        {
            Assert.Equal("asset_status", AssetStatusPickList.PickListName);
            Assert.Equal(100, AssetStatusPickList.Values.Draft);
            Assert.Equal(200, AssetStatusPickList.Values.Active);
            Assert.Equal("Active", AssetStatusPickList.Names.Active);
            Assert.Equal("Under Maintenance", AssetStatusPickList.GetName(300));
            Assert.Equal("Active", AssetStatusPickList.GetName(AssetStatusEnum.Active));
        }

        [Fact]
        public void UserDtoAndUserEntity_IntegrateGeneratedUserRolesEnum()
        {
            UserEntity userEntity = new UserEntity
            {
                Email = "admin@assetmgmt.io",
                Role = (int)UserRolesEnum.Administrator
            };

            Assert.Equal(UserRolesEnum.Administrator, userEntity.RoleEnum);
            Assert.Equal("Administrator", userEntity.RoleName);

            UserDto userDto = new UserDto(
                "usr-1", "Jane", "Doe", "Jane Doe", "admin@assetmgmt.io",
                (int)UserRolesEnum.ComplianceOfficer, "local", null, "en-US", DateTime.UtcNow);

            Assert.Equal(UserRolesEnum.ComplianceOfficer, userDto.RoleEnum);
            Assert.Equal("Compliance Officer", userDto.RoleName);

            UserBootstrapDto bootstrapDto = new UserBootstrapDto
            {
                Role = (int)UserRolesEnum.AssetManager
            };

            Assert.Equal(UserRolesEnum.AssetManager, bootstrapDto.RoleEnum);
            Assert.Equal("Asset Manager", bootstrapDto.RoleName);
        }
    }
}
