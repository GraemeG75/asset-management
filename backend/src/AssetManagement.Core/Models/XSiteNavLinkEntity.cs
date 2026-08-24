using System;

namespace AssetManagement.Core.Models
{
    public class XSiteNavLinkEntity : IAuditEntity
    {
        public Guid NavId { get; set; }
        public string LinkKey { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public int? BadgeCount { get; set; }
        public string Category { get; set; } = "Main";
        public int DisplayOrder { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
