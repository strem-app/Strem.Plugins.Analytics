using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Publish;
using Cake.Frosting;

namespace Strem.Build.Tasks;

[IsDependentOn(typeof(CleanDirectoriesTask))]
public class PackageAnalyticsViewerPluginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var pluginName = $"Strem.Plugins.Analytics.Viewer";
        var pluginTempOutput = $"{Directories.Dist}/plugin-temp";
        var pluginContainerFolder = $"{Directories.Dist}/plugin/";
        var pluginFinalOutput = $"{pluginContainerFolder}/{pluginName}";
        if(!Directory.Exists(pluginTempOutput)) { context.CreateDirectory(pluginTempOutput); }
        if(!Directory.Exists(pluginFinalOutput)) { context.CreateDirectory(pluginFinalOutput); }

        var pluginProject = $"{Directories.Src}/Strem.Plugins.Analytics.Viewer/Strem.Plugins.Analytics.Viewer.csproj";
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

        context.MoveFile($"{pluginTempOutput}/Strem.Plugins.Analytics.Viewer.dll", $"{pluginFinalOutput}/Strem.Plugins.Analytics.Viewer.dll");
        context.MoveFile($"{pluginTempOutput}/Blazor-ApexCharts.dll", $"{pluginFinalOutput}/Blazor-ApexCharts.dll");
        context.MoveDirectory($"{pluginTempOutput}/wwwroot", $"{pluginFinalOutput}/wwwroot");
        context.Zip(pluginContainerFolder, $"{Directories.Dist}/{pluginName}.zip");
    }
}