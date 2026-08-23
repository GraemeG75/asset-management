namespace AssetManagement.Core.Models
{
    public class SearchConfigDto
    {
        public string? TargetGridId { get; set; }
        public bool AutoSubmitOnReset { get; set; } = true;
        public string SubmitButtonLabel { get; set; } = "Filter Results";
    }
}
