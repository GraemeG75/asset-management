namespace AssetManagement.Core.Models
{
    public class DashboardFormMetadataDto
    {
        public string FormId { get; set; } = string.Empty;
        public string FormType { get; set; } = "standard"; // "widget", "detail", "grid", "search", "standard"
        public string Caption { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? FormInfo { get; set; }
        public bool IsEditable { get; set; } = true;
        public string? LabelPosition { get; set; }
        public int GridCols { get; set; } = 12;
        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
        public WidgetConfigDto? WidgetConfig { get; set; }
        public GridConfigDto? GridConfig { get; set; }
        public SearchConfigDto? SearchConfig { get; set; }
        public string SubmitButtonText { get; set; } = "Submit";
        public bool ShowResetButton { get; set; } = true;
    }
}
