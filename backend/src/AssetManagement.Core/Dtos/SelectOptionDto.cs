namespace AssetManagement.Core.Dtos
{
    public class SelectOptionDto
    {
        public string Label { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
        public bool Disabled { get; set; }
    }
}
