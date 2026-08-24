using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class FormFieldDto
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = "text";
        public string? Placeholder { get; set; }
        public object? DefaultValue { get; set; }
        public object? Value { get; set; }
        public string LabelPosition { get; set; } = "top";
        public List<SelectOptionDto>? Options { get; set; }
        public List<FieldValidatorDto>? Validators { get; set; }
        public bool Disabled { get; set; }
        public bool Readonly { get; set; }
        public string? HelpText { get; set; }
        public int GridCols { get; set; } = 12;
        public string? CustomCssClass { get; set; }
    }
}
