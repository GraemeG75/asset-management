using System.Collections.Generic;

namespace AssetManagement.Core.Dtos
{
    public class GridConfigDto
    {
        public int PageSize { get; set; } = 10;
        public bool AllowSorting { get; set; } = true;
        public bool AllowPaging { get; set; } = true;
        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();
    }
}
