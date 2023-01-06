using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Viewer.Components.General;

namespace Strem.Plugins.Analytics.Viewer.Plugin.ComponentDescriptors;

public class UserInteractionsChartDescriptor : IAnalyticsComponentDescriptor
{
    public string Title => "User Interactions Chart";
    public string Code => "general-user-interactions-chart";
    public Type ComponentType => typeof(UserInteractionsChart);
    public bool CanBeShown(AnalyticsFilter filter) => true;
}