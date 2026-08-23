namespace AssetManagement.Core.Models
{
    public class FormSchemaDto
    {
        public string Id { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? FormInfo { get; set; }
        public bool IsEditable { get; set; } = true;
        public string? LabelPosition { get; set; }
        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
        public string SubmitButtonText { get; set; } = "Submit";
        public bool ShowResetButton { get; set; } = true;
    }
}
