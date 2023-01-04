using Strem.Core.Variables;

namespace Strem.Plugins.Analytics.Twitch.Variables;

public class TwitchAnalyticsViewerVars
{
    // Generic
    public static readonly string AnalyticsTwitchContext = "analytics.twitch";
    
    // OAuth (app)
    public static readonly VariableEntry Channels = new("channels", AnalyticsTwitchContext);
}