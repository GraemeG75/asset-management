namespace AssetManagement.Core.Dtos
{
    public class WidgetConfigDto
    {
        public string WidgetType { get; set; } = "kpi";
        public string? MetricValue { get; set; }
        public string? MetricTrend { get; set; }
        public string TrendDirection { get; set; } = "neutral";
        public string AccentColor { get; set; } = "#3B82F6";
        public string? Icon { get; set; }
    }
}
