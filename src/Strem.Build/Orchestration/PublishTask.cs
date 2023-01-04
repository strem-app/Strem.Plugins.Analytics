using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish")]
[IsDependentOn(typeof(PackageAnalyticsPluginTask))]
[IsDependentOn(typeof(PackageAnalyticsTwitchPluginTask))]
[IsDependentOn(typeof(PackageAnalyticsViewerPluginTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    
}