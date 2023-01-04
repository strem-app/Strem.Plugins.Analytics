using Strem.Core.Variables;

namespace Strem.Plugins.Analytics.Services.Integrations;

public interface IAnalyticsIntegrationDescriptor
{
    public string Title { get; }
    public string Code { get; }
    public VariableDescriptor[] VariableOutputs { get; }
    public Type ComponentType { get; }
}