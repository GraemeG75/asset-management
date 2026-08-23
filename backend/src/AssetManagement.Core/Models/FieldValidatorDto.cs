namespace AssetManagement.Core.Models
{
    public class FieldValidatorDto
    {
        public string Type { get; set; } = string.Empty;
        public object? Value { get; set; }
        public string? Message { get; set; }
    }
}
