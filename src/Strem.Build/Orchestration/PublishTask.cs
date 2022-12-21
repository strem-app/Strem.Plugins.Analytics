using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("publish")]
[IsDependentOn(typeof(PackageAnalyticsPluginTask))]
[IsDependentOn(typeof(PackageAnalyticsTwitchPluginTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    
}