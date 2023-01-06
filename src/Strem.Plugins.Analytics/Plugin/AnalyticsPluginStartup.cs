using System.Reactive.Disposables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistity.Encryption;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Plugins.Analytics.Services.Database;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Services.Settings;

namespace Strem.Plugins.Analytics.Plugin;

public class AnalyticsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public IEncryptor Encryptor { get; }
    public IAnalyticsSettingsRegistry AnalyticsSettingsRegistry { get; }
    public IAnalyticsComponentRegistry AnalyticsComponentsRegistry { get; }
    public IAnalyticsDatabase AnalyticsDatabase { get; }
    public IServiceProvider Services { get; }
    public ILogger<AnalyticsPluginStartup> Logger { get; }
    
    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public AnalyticsPluginStartup(IEventBus eventBus, IAppState appState, IEncryptor encryptor, IAnalyticsSettingsRegistry analyticsSettingsRegistry, IAnalyticsDatabase analyticsDatabase, IServiceProvider services, ILogger<AnalyticsPluginStartup> logger, IAnalyticsComponentRegistry analyticsComponentsRegistry)
    {
        EventBus = eventBus;
        AppState = appState;
        Encryptor = encryptor;
        AnalyticsSettingsRegistry = analyticsSettingsRegistry;
        AnalyticsDatabase = analyticsDatabase;
        Services = services;
        Logger = logger;
        AnalyticsComponentsRegistry = analyticsComponentsRegistry;
    }

    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {
        SetupRegistries();

        EventBus.Receive<ApplicationClosingEvent>()
            .Subscribe(x => OnAppClosing())
            .AddTo(_subs);
    }

    private void OnAppClosing()
    {
        AnalyticsDatabase?.Dispose();
    }

    public void SetupRegistries()
    {
        var integrationDescriptors = Services.GetServices<IAnalyticsSettingsDescriptor>();
        AnalyticsSettingsRegistry?.AddMany(integrationDescriptors);

        var componentDescriptors = Services.GetServices<IAnalyticsComponentDescriptor>();
        AnalyticsComponentsRegistry?.AddMany(componentDescriptors);
    }
    
    public void Dispose()
    { _subs?.Dispose(); }
}