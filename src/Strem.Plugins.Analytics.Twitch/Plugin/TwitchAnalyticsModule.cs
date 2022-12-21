using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Flows.Extensions;

namespace Strem.Plugins.Analytics.Twitch.Plugin;

public class TwitchAnalyticsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, TwitchAnalyticsPluginStartup>();
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
        
    }
}