namespace AssetManagement.Core.Models
{
    public class ProfileNavLinkDto
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Badge { get; set; }
        public string? BadgeColor { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
