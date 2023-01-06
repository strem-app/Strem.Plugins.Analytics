using Strem.Core.Services.Registries;

namespace Strem.Plugins.Analytics.Services.Settings;

public class AnalyticsSettingsRegistry : Registry<IAnalyticsSettingsDescriptor>, IAnalyticsSettingsRegistry
{
    public override string GetId(IAnalyticsSettingsDescriptor data) => data.Code;
}