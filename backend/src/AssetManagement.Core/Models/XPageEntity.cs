using System;

namespace AssetManagement.Core.Models
{
    public class XPageEntity : IAuditEntity
    {
        public Guid PageGuid { get; set; }
        public string PageKey { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
