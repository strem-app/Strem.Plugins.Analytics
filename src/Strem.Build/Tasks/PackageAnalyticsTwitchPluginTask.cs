using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
public class PackageAnalyticsTwitchPluginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var pluginName = $"Strem.Plugins.Analytics.Twitch";
        var pluginTempOutput = $"{Directories.Dist}/plugin-temp";
        var pluginContainerFolder = $"{Directories.Dist}/plugin/";
        var pluginFinalOutput = $"{pluginContainerFolder}/{pluginName}";
        if(!Directory.Exists(pluginTempOutput)) { context.CreateDirectory(pluginTempOutput); }
        if(!Directory.Exists(pluginFinalOutput)) { context.CreateDirectory(pluginFinalOutput); }

        var pluginProject = $"{Directories.Src}/Strem.Plugins.Analytics.Twitch/Strem.Plugins.Analytics.Twitch.csproj";
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

        context.MoveFile($"{pluginTempOutput}/Strem.Plugins.Analytics.Twitch.dll", $"{pluginFinalOutput}/Strem.Plugins.Analytics.Twitch.dll");
        context.Zip(pluginContainerFolder, $"{Directories.Dist}/{pluginName}.zip");
    }
}