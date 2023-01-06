namespace Strem.Plugins.Analytics.Models;

public record AnalyticsEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string EventType { get; set; }
    public DateTime EventDateTime { get; set; }
    public decimal EventValue { get; set; }
    
    public string UserContext { get; set; }
    public string SourceContext { get; set; }
    public string PlatformContext { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}