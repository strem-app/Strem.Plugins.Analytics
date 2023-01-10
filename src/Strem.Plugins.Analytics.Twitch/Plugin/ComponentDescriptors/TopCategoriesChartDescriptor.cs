using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Twitch.Components.Analytics;

namespace Strem.Plugins.Analytics.Twitch.Plugin.ComponentDescriptors;

public class TopCategoriesChartDescriptor : IAnalyticsComponentDescriptor
{
    public string Title => "Top Categories Chart";
    public string Code => "twitch-top-categories-chart";
    public Type ComponentType => typeof(TopCategoriesChart);
    public bool CanBeShown(AnalyticsFilter filter) => filter.PlatformContext.Equals("twitch", StringComparison.OrdinalIgnoreCase);
}