namespace AssetManagement.Core.Models
{
    public class XProfileNavLinkEntity
    {
        public Guid NavId { get; set; }
        public string LinkKey { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Badge { get; set; }
        public string? BadgeColor { get; set; }
        public int DisplayOrder { get; set; }
    }
}
