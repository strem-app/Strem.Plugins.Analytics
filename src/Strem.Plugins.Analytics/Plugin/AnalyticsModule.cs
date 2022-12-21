using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Menus;
using Strem.Data.Types;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services;
using Strem.Plugins.Analytics.Services.Database;
using Strem.Plugins.Analytics.Services.Repositories;

namespace Strem.Plugins.Analytics.Plugin;

public class AnalyticsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Menus
        services.AddSingleton(new MenuDescriptor
        {
            Priority = 5,
            Title = "Analytics",
            Code = "analytics-menu",
            IconClass = "fas fa-chart-line",
            PageUrl = "analytics"
        });
        
        // Plugin
        services.AddSingleton<IPluginStartup, AnalyticsPluginStartup>();
        
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
        services.AddSingleton<IStreamMetricRepository, StreamMetricRepository>();
        services.AddSingleton<IStreamInteractionRepository, StreamInteractionRepository>();
    }
}