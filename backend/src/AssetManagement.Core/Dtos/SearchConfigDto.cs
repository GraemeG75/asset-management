namespace AssetManagement.Core.Dtos
{
    public class SearchConfigDto
    {
        public string? TargetGridId { get; set; }
        public bool AutoSubmitOnReset { get; set; }
        public string SubmitButtonLabel { get; set; } = "Search";
    }
}
