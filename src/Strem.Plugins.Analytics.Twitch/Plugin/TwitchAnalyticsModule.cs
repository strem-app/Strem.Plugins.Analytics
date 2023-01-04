using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Flows.Extensions;
using Strem.Plugins.Analytics.Services.Integrations;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitchAnalyticsPluginStartup>();
        
        // Analytics Integrations
        services.AddSingleton<IAnalyticsIntegrationDescriptor, TwitchAnalyticsIntegrationDescriptor>();
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
    }
}