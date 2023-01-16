using System.Text;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Viewer.Extensions;

public static class FilterExtensions
{
    public static string ToWhereClause(this AnalyticsFilter filter)
    {
        var whereBuilder = new StringBuilder();
        whereBuilder.AppendLine($"$.EventDateTime < '{filter.EndPeriod.ToString("O")}'");
        whereBuilder.AppendLine($"AND $.EventDateTime > '{filter.StartPeriod.ToString("O")}'");
        
        if (!string.IsNullOrEmpty(filter.PlatformContext)) 
        { whereBuilder.AppendLine($"AND $.PlatformContext = '{filter.PlatformContext}'"); }
        
        if (!string.IsNullOrEmpty(filter.SourceContext)) 
        { whereBuilder.AppendLine($"AND $.SourceContext = '{filter.SourceContext}'"); }

        return whereBuilder.ToString();
    }
}