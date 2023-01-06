using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services;
using Strem.Plugins.Analytics.Services.Database;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Services.Repositories;
using Strem.Plugins.Analytics.Services.Settings;

namespace Strem.Plugins.Analytics.Plugin;

public class AnalyticsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Plugin
        services.AddSingleton<IPluginStartup, AnalyticsPluginStartup>();
        
        // Analytics registry
        services.AddSingleton<IAnalyticsSettingsRegistry, AnalyticsSettingsRegistry>();
        services.AddSingleton<IAnalyticsComponentRegistry, AnalyticsComponentRegistry>();
        
        // Database
        SetupDatabase(services);
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
    
    public void SetupDatabase(IServiceCollection services)
    {
        if (!Directory.Exists(StremPathHelper.StremDataDirectory))
        { Directory.CreateDirectory(StremPathHelper.StremDataDirectory); }
        
        var profile = "analytics";
        var dbPath = $"{StremPathHelper.StremDataDirectory}/{profile}.db";
        services.AddSingleton<IAnalyticsDatabase>(x => new AnalyticsDatabase(dbPath));
        services.AddSingleton<IAnalyticsEventRepository, AnalyticsEventRepository>();
    }
}