using System.Collections.Generic;
using AssetManagement.Core.Generated.PickLists;

namespace AssetManagement.Core.Dtos
{
    public class UserBootstrapDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Role { get; set; } = (int)UserRolesEnum.AssetManager;
        public UserRolesEnum RoleEnum => (UserRolesEnum)Role;
        public string RoleName => UserRolesPickList.GetName(Role);
        public List<ProfileNavLinkDto> ProfileNavLinks { get; set; } = new List<ProfileNavLinkDto>();
        public List<SiteNavLinkDto> SiteNavLinks { get; set; } = new List<SiteNavLinkDto>();
        public int InboxCount { get; set; }
        public List<DashboardFormMetadataDto> DashboardForms { get; set; } = new List<DashboardFormMetadataDto>();
    }
}
