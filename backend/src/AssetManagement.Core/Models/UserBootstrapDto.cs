namespace AssetManagement.Core.Models
{
    public class UserBootstrapDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Role { get; set; } = "Asset Manager";
        public List<ProfileNavLinkDto> ProfileNavLinks { get; set; } = new List<ProfileNavLinkDto>();
        public List<SiteNavLinkDto> SiteNavLinks { get; set; } = new List<SiteNavLinkDto>();
        public int InboxCount { get; set; }
        public List<DashboardFormMetadataDto> DashboardForms { get; set; } = new List<DashboardFormMetadataDto>();
    }
}
