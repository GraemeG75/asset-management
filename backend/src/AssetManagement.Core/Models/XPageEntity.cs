namespace AssetManagement.Core.Models
{
    public class XPageEntity
    {
        public Guid PageGuid { get; set; }
        public string PageKey { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
    }
}
