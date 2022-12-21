using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Persistity.Encryption;
using Strem.Core.Events.Bus;
using Strem.Core.Plugins;
using Strem.Core.State;

namespace Strem.Plugins.Analytics.Plugin;

public class AnalyticsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IEncryptor Encryptor { get; }
    public ILogger<AnalyticsPluginStartup> Logger { get; }
    
    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public AnalyticsPluginStartup(IEventBus eventBus, IAppState appState, ILogger<AnalyticsPluginStartup> logger, IEncryptor encryptor)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
        Encryptor = encryptor;
    }
    
    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {

    }

    public void Dispose()
    { _subs?.Dispose(); }
}