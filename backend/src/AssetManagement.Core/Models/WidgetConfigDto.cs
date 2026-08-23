namespace AssetManagement.Core.Models
{
    public class WidgetConfigDto
    {
        public string WidgetType { get; set; } = "kpi";
        public string? MetricValue { get; set; }
        public string? MetricTrend { get; set; }
        public string TrendDirection { get; set; } = "neutral";
        public string AccentColor { get; set; } = "#3b82f6";
        public string? Icon { get; set; }
    }
}
