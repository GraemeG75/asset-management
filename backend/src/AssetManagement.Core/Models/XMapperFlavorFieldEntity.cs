using System;

namespace AssetManagement.Core.Models
{
    public class XMapperFlavorFieldEntity : IAuditEntity
    {
        public Guid FlavorFieldGuid { get; set; }
        public Guid FlavorId { get; set; }
        public string FlavorKey { get; set; } = string.Empty;
        public Guid? MapperFieldId { get; set; }
        public string? MapperFieldName { get; set; }
        public string KeyName { get; set; } = string.Empty;
        public string FieldType { get; set; } = "text";
        public bool IsEditable { get; set; } = true;
        public bool IsReadonly { get; set; }
        public bool IsDisabled { get; set; }
        public string RequestedLocale { get; set; } = "en-US";
        public string Label { get; set; } = string.Empty;
        public string? Placeholder { get; set; }
        public string? DefaultValue { get; set; }
        public string? HelpText { get; set; }
        public int DisplayOrder { get; set; }
        public int GridCols { get; set; } = 12;
        public string? CustomCssClass { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
