using System;

namespace AssetManagement.Core.Models
{
    public class XFormEntity : IAuditEntity
    {
        public Guid FormGuid { get; set; }
        public string FormKey { get; set; } = string.Empty;
        public Guid? FlavorId { get; set; }
        public string? FlavorKey { get; set; }
        public string? FlavorDisplayName { get; set; }
        public string FormType { get; set; } = "standard";
        public string? VisibleClause { get; set; }
        public string RequestedLocale { get; set; } = "en-US";
        public string Caption { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FormInfo { get; set; }
        public string SubmitButtonText { get; set; } = "Submit";
        public bool IsEditable { get; set; } = true;
        public string LabelPosition { get; set; } = "left";
        public int GridCols { get; set; } = 12;
        public bool ShowResetButton { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? CreatedById { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}
