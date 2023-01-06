namespace Strem.Plugins.Analytics.Types;

public class AnalyticsEventTypes
{
    public static readonly string Unknown = string.Empty;

    // Interaction Events
    public static readonly string ChatMessage = "chat-message";
    public static readonly string UserJoined = "user-joined";
    public static readonly string UserLeft = "user-left";
    
    // Metric Events
    public static readonly string ViewerCount = "viewer-count";
    public static readonly string Currency = "currency";
}