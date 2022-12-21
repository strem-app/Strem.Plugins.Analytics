using Cake.Frosting;
using Strem.Build.Tasks;

namespace Strem.Build.Orchestration;

[TaskName("default")]
[IsDependentOn(typeof(BuildAndTestTask))]
[IsDependentOn(typeof(PublishTask))]
public class DefaultTask : FrostingTask<BuildContext>
{
    
}