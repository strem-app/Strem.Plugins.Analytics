using Strem.Core.Services.Registries;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Services.Metrics;

public interface IAnalyticsComponentRegistry : IRegistry<IAnalyticsComponentDescriptor>
{
    IEnumerable<IAnalyticsComponentDescriptor> GetApplicable(AnalyticsFilter filter);
}