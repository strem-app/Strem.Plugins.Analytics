using Microsoft.Extensions.Logging;
using Moq;
using Persistity.Encryption;
using Strem.Core.Events.Bus;
using Strem.Core.State;
using Strem.Plugins.Analytics.Plugin;
using Strem.Plugins.Analytics.Services.Database;
using Strem.Plugins.Analytics.Services.Metrics;
using Strem.Plugins.Analytics.Services.Settings;

namespace Strem.Plugins.Analytics.UnitTests.Analytics;

public class AnalyticsPluginStartupTests
{
    [Fact]
    public async Task should_correctly_setup_plugin()
    {
        var mockEventBus = new Mock<IEventBus>();
        var mockAppState = new Mock<IAppState>();
        var mockEncryptor = new Mock<IEncryptor>();
        var mockAnalyticsRegistry = new Mock<IAnalyticsSettingsRegistry>();
        var mockAnalyticsDatabase = new Mock<IAnalyticsDatabase>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockLogger = new Mock<ILogger<AnalyticsPluginStartup>>();
        var mockAnalyticsComponentRegistry = new Mock<IAnalyticsComponentRegistry>();

        var analyticsPluginStartup = new AnalyticsPluginStartup(mockEventBus.Object, mockAppState.Object,
            mockEncryptor.Object, mockAnalyticsRegistry.Object, mockAnalyticsDatabase.Object,
            mockServiceProvider.Object, mockLogger.Object, mockAnalyticsComponentRegistry.Object);

        await analyticsPluginStartup.SetupPlugin();
    }
}