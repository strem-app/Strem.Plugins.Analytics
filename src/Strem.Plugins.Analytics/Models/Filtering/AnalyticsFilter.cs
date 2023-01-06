namespace Strem.Plugins.Analytics.Models.Filtering;

public class AnalyticsFilter
{
    public string SourceContext { get; set; } = string.Empty;
    public string PlatformContext { get; set; } = string.Empty;
    public DateTime StartPeriod { get; set; }
    public DateTime EndPeriod { get; set; }
    public TimeUnitRounding TimeUnitRounding { get; set; } = new();

    public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
}