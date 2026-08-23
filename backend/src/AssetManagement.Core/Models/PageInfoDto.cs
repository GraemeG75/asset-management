namespace AssetManagement.Core.Models
{
    public class PageInfoDto
    {
        public string PageId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<PageFormSummaryDto> Forms { get; set; } = new List<PageFormSummaryDto>();
    }
}
