using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Plugins;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Services.Registries.Menus;
using Strem.Flows.Extensions;
using Strem.Infrastructure.Services;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Viewer.Components.General;
using Strem.Plugins.Analytics.Viewer.Plugin.ComponentDescriptors;

namespace Strem.Plugins.Analytics.Viewer.Plugin;

public class AnalyticsViewerModule : IDependencyModule
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
        services.AddSingleton<IPluginStartup, AnalyticsViewerPluginStartup>();
        
        // Integration Components
        services.AddSingleton<IIntegrationDescriptor, AnalyticsViewerIntegrationDescriptor>();
        
        // Analytics Components
        services.AddSingleton<IAnalyticsComponentDescriptor, TopChattersDescriptor>();
        services.AddSingleton<IAnalyticsComponentDescriptor, UserInteractionsChartDescriptor>();
        
        // Register Components
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
}