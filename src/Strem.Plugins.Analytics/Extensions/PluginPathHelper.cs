namespace Strem.Plugins.Analytics.Extensions;

public static class PluginPathHelper
{
    public static string GetPluginStaticFileRootPath(string pluginName)
    {
        return $"http://localhost:30212/Plugins/{pluginName}/wwwroot";
    } 
}