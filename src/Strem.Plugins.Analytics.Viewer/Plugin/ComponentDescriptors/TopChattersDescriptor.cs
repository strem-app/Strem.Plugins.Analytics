using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Viewer.Components.Analytics;

namespace Strem.Plugins.Analytics.Viewer.Plugin.ComponentDescriptors;

public class TopChattersDescriptor : IAnalyticsComponentDescriptor
{
    public string Title => "Top Chatters";
    public string Code => "general-top-chatters";
    public Type ComponentType => typeof(TopChattersChart);
    public bool CanBeShown(AnalyticsFilter filter) => true;
}