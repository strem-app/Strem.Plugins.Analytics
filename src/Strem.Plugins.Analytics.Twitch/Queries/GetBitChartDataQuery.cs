using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Twitch.Models;
using Strem.Plugins.Analytics.Twitch.Types;
using Strem.Plugins.Analytics.Viewer.Extensions;

namespace Strem.Plugins.Analytics.Twitch.Queries;

public class GetBitChartDataQuery : IRawQuery<IEnumerable<BitsChartMetrics>>
{
    public AnalyticsFilter Filter { get; }
    
    private static string InternalQuery = $@"SELECT {{
            {nameof(BitsChartMetrics.Date)}: @key, 
            {nameof(BitsChartMetrics.Viewers)}: AVG(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{TwitchAnalyticsEventTypes.ViewerCount}') => @.value)
            ),
            {nameof(BitsChartMetrics.Bits)}: SUM(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{TwitchAnalyticsEventTypes.Bits}') => @.value)
            ) 
        }}
        FROM analytics_events";

    public GetBitChartDataQuery(AnalyticsFilter filter)
    {
        Filter = filter;
    }

    public string GetFilteredQuery()
    {
        return $@"{InternalQuery}
            WHERE ($.EventType = '{TwitchAnalyticsEventTypes.Bits}' OR $.EventType = '{TwitchAnalyticsEventTypes.ViewerCount}')
            AND {Filter.ToWhereClause()}
            GROUP BY SUBSTRING($.EventDateTime, 0, {Filter.TimeUnitRounding.ToSubStringAccuracy()})";
    }

    public IEnumerable<BitsChartMetrics> Query(ILiteDatabase connection)
    {
        using var reader = connection.Execute(GetFilteredQuery());
        return reader
            .ToArray()
            .Select(x => new BitsChartMetrics(
                DateTime.Parse(x[nameof(BitsChartMetrics.Date)]), 
               (int)x[nameof(BitsChartMetrics.Viewers)].AsDouble, 
                (int)x[nameof(BitsChartMetrics.Bits)].AsDouble));
    }
}