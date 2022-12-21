namespace Strem.Plugins.Analytics.Models;

public class StreamInteraction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string InteractionType { get; set; }
    public DateTime InteractionDateTime { get; set; }
    
    public string UserContext { get; set; }
    public string SourceContext { get; set; }
    public string PlatformContext { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}