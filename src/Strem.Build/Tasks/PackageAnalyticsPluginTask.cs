using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
public class PackageAnalyticsPluginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var pluginName = $"Strem.Plugins.Analytics";
        var pluginTempOutput = $"{Directories.Dist}/plugin-temp";
        var pluginContainerFolder = $"{Directories.Dist}/plugin/";
        var pluginFinalOutput = $"{pluginContainerFolder}/{pluginName}";
        if(!Directory.Exists(pluginTempOutput)) { context.CreateDirectory(pluginTempOutput); }
        if(!Directory.Exists(pluginFinalOutput)) { context.CreateDirectory(pluginFinalOutput); }

        var pluginProject = $"{Directories.Src}/Strem.Plugins.Analytics/Strem.Plugins.Analytics.csproj";
        var publishSettings = new DotNetPublishSettings
        {
            Configuration = "Release",
            OutputDirectory = pluginTempOutput,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                Version = context.Version
            }
        };
        context.DotNetPublish(pluginProject, publishSettings);

        context.MoveFile($"{pluginTempOutput}/Strem.Plugins.Analytics.dll", $"{pluginFinalOutput}/Strem.Plugins.Analytics.dll");
        context.Zip(pluginContainerFolder, $"{Directories.Dist}/{pluginName}.zip");
    }
}