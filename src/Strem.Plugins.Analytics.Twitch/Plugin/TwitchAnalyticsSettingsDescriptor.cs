using Strem.Core.Extensions;
using Strem.Core.Variables;
using Strem.Plugins.Analytics.Services.Settings;
using Strem.Plugins.Analytics.Twitch.Components.Integrations;
using Strem.Plugins.Analytics.Twitch.Variables;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsSettingsDescriptor : IAnalyticsSettingsDescriptor
{
    public string Title => "Twitch Analytics Settings";
    public string Code => "twitch-analytics-settings";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        TwitchAnalyticsViewerVars.Channels.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(TwitchAnalyticsSettingsComponent);
}