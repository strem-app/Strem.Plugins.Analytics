using Strem.Plugins.Analytics.Components;

namespace Strem.Plugins.Analytics.Viewer.Components;

public abstract class ChartComponent : AnalyticsComponent
{
    public abstract Task Refresh();
}