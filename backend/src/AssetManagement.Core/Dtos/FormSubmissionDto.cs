using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class FormSubmissionDto
    {
        public string PageKey { get; set; } = string.Empty;
        public string FormKey { get; set; } = string.Empty;
        public string FormType { get; set; } = "standard"; // 'standard', 'detail', 'grid', 'search', 'widget'
        public string? RecordId { get; set; }
        public string Action { get; set; } = "save"; // 'save', 'create', 'update', 'delete', 'search', 'widget-action'
        public Dictionary<string, object?> FieldValues { get; set; } = new Dictionary<string, object?>();
    }
}
