using Microsoft.AspNetCore.Components;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Components;

public class AnalyticsComponent : ComponentBase
{
    [Parameter]
    public IReadOnlyCollection<AnalyticsEvent> Data { get; set; }
    
    [Parameter]
    public AnalyticsFilter Filter { get; set; }
}