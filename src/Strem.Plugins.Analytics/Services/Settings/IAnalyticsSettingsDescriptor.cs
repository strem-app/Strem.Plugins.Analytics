using Strem.Core.Variables;

namespace Strem.Plugins.Analytics.Services.Settings;

public interface IAnalyticsSettingsDescriptor
{
    public string Title { get; }
    public string Code { get; }
    public VariableDescriptor[] VariableOutputs { get; }
    public Type ComponentType { get; }
}