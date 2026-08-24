using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class DashboardFormMetadataDto
    {
        public string FormId { get; set; } = string.Empty;
        public string FormType { get; set; } = "standard";
        public string Caption { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FormInfo { get; set; }
        public bool IsEditable { get; set; } = true;
        public string LabelPosition { get; set; } = "top";
        public int GridCols { get; set; } = 2;
        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
        public WidgetConfigDto? WidgetConfig { get; set; }
        public GridConfigDto? GridConfig { get; set; }
        public SearchConfigDto? SearchConfig { get; set; }
        public string SubmitButtonText { get; set; } = "Save";
        public bool ShowResetButton { get; set; }
    }
}
