namespace AssetManagement.Core.Models
{
    public class SelectOptionDto
    {
        public string Label { get; set; } = string.Empty;
        public object? Value { get; set; }
        public bool Disabled { get; set; }
    }
}
