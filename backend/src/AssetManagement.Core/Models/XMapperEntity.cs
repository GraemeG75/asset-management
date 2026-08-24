using System;

namespace AssetManagement.Core.Models
{
    public class XMapperEntity : IAuditEntity
    {
        public Guid MapperGuid { get; set; }
        public string MapperKey { get; set; } = string.Empty;
        public string SourceType { get; set; } = "TABLE";
        public string SourceName { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
