using Strem.Core.Services.Registries;

namespace Strem.Plugins.Analytics.Services.Integrations;

public class AnalyticsIntegrationRegistry : Registry<IAnalyticsIntegrationDescriptor>, IAnalyticsIntegrationRegistry
{
    public override string GetId(IAnalyticsIntegrationDescriptor data) => data.Code;
}