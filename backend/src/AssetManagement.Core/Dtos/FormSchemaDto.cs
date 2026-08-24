using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class FormSchemaDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FormInfo { get; set; }
        public bool IsEditable { get; set; } = true;
        public string LabelPosition { get; set; } = "top";
        public List<FormFieldDto> Fields { get; set; } = new List<FormFieldDto>();
        public string SubmitButtonText { get; set; } = "Save";
        public bool ShowResetButton { get; set; }
    }
}
