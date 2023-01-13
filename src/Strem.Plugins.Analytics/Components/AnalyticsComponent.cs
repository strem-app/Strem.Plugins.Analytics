using Microsoft.AspNetCore.Components;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Services.Repositories;

namespace Strem.Plugins.Analytics.Components;

public class AnalyticsComponent : ComponentBase
{
    [Inject]
    public IAnalyticsEventRepository AnalyticsEventRepository { get; set; }
    
    [Parameter]
    public AnalyticsFilter Filter { get; set; }
}