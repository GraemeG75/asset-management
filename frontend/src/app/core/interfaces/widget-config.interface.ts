export interface WidgetConfig {
  widgetType: 'kpi' | 'chart' | 'action';
  metricValue?: string;
  metricTrend?: string;
  trendDirection: 'up' | 'down' | 'neutral';
  accentColor: string;
  icon?: string;
}
