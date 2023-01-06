using Strem.Core.Services.Registries;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Services.Metrics;

public class AnalyticsComponentRegistry : Registry<IAnalyticsComponentDescriptor>, IAnalyticsComponentRegistry
{
    public override string GetId(IAnalyticsComponentDescriptor data) => data.Code;
    
    public IEnumerable<IAnalyticsComponentDescriptor> GetApplicable(AnalyticsFilter filter) => Data.Values.Where(x => x.CanBeShown(filter));
}