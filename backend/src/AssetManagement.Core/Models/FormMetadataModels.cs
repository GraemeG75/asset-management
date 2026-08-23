namespace AssetManagement.Core.Models;

public class PageFormSummaryDto
{
    public string FormId { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PageInfoDto
{
    public string PageId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PageFormSummaryDto> Forms { get; set; } = new();
}

public class SelectOptionDto
{
    public string Label { get; set; } = string.Empty;
    public object? Value { get; set; }
    public bool Disabled { get; set; }
}

public class FieldValidatorDto
{
    public string Type { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string? Message { get; set; }
}

public class FormFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public string? Placeholder { get; set; }
    public object? DefaultValue { get; set; }
    public string? LabelPosition { get; set; }
    public List<SelectOptionDto>? Options { get; set; }
    public List<FieldValidatorDto>? Validators { get; set; }
    public bool Disabled { get; set; }
    public bool Readonly { get; set; }
    public string? HelpText { get; set; }
    public int GridCols { get; set; } = 12;
    public string? CustomCssClass { get; set; }
}

public class FormSchemaDto
{
    public string Id { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FormInfo { get; set; }
    public bool IsEditable { get; set; } = true;
    public string? LabelPosition { get; set; }
    public List<FormFieldDto> Fields { get; set; } = new();
    public string SubmitButtonText { get; set; } = "Submit";
    public bool ShowResetButton { get; set; } = true;
}
