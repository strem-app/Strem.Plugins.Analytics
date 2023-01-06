using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Twitch.Components.Analytics;

namespace Strem.Plugins.Analytics.Twitch.Plugin.ComponentDescriptors;

public class BitsChartDescriptor : IAnalyticsComponentDescriptor
{
    public string Title => "Bits Spent Chart";
    public string Code => "twitch-bits-spent-chart";
    public Type ComponentType => typeof(BitsChart);
    public bool CanBeShown(AnalyticsFilter filter) => filter.PlatformContext.Equals("twitch", StringComparison.OrdinalIgnoreCase);
}