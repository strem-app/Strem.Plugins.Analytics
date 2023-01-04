using Strem.Core.Extensions;
using Strem.Core.Variables;
using Strem.Plugins.Analytics.Services.Integrations;
using Strem.Plugins.Analytics.Twitch.Components.Integrations;
using Strem.Plugins.Analytics.Twitch.Variables;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsIntegrationDescriptor : IAnalyticsIntegrationDescriptor
{
    public string Title => "Twitch Analytics Integration";
    public string Code => "twitch-analytics-integration";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TwitchAnalyticsViewerVars.Channels.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(TwitchAnalyticsIntegrationComponent);
}