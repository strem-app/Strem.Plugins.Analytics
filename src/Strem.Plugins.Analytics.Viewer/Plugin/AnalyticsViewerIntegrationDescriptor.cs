using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.Plugins.Analytics.Viewer.Components.Integrations;

namespace Strem.Plugins.Analytics.Viewer.Plugin;

public class AnalyticsViewerIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "Analytics Viewer Integration";
    public string Code => "analytics-viewer-integration";

    public VariableDescriptor[] VariableOutputs { get; } = Array.Empty<VariableDescriptor>();

    public Type ComponentType { get; } = typeof(AnalyticsViewerIntegrationComponent);
}