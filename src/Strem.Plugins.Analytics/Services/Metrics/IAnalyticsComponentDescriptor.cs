using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Services.Metrics;

public interface IAnalyticsComponentDescriptor
{
    public string Title { get; }
    public string Code { get; }
    public Type ComponentType { get; }
    
    public bool CanBeShown(AnalyticsFilter filter);
}