using System;

namespace AssetManagement.Core.Models
{
    public class XMapperFlavorEntity : IAuditEntity
    {
        public Guid FlavorGuid { get; set; }
        public string FlavorKey { get; set; } = string.Empty;
        public Guid MapperId { get; set; }
        public string MapperKey { get; set; } = string.Empty;
        public string RequestedLocale { get; set; } = "en-US";
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
