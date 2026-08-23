namespace AssetManagement.Core.Models
{
    public class SiteNavLinkDto
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int? BadgeCount { get; set; }
        public string Category { get; set; } = "Main";
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
