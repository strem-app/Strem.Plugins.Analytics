namespace Strem.Plugins.Analytics.Models;

public class StreamMetric
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string MetricType { get; set; }
    public DateTime MetricDateTime { get; set; }
    public decimal MetricValue { get; set; }
    
    public string UserContext { get; set; }
    public string SourceContext { get; set; }
    public string PlatformContext { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}