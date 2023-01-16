using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Twitch.Models;
using Strem.Plugins.Analytics.Twitch.Types;
using Strem.Plugins.Analytics.Viewer.Extensions;

namespace Strem.Plugins.Analytics.Twitch.Queries;

public class GetTopCategoriesQuery : IRawQuery<IEnumerable<CategoryChartMetric>>
{
    public AnalyticsFilter Filter { get; }
    
    private static string InternalQuery = $@"SELECT {{
            {nameof(CategoryChartMetric.Category)}: @key, 
            {nameof(CategoryChartMetric.Viewers)}: AVG(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{TwitchAnalyticsEventTypes.ViewerCount}') => @.value)
            ), 
            {nameof(CategoryChartMetric.Bits)}: SUM(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{TwitchAnalyticsEventTypes.Bits}') => @.value)
            ),
            {nameof(CategoryChartMetric.Subs)}: SUM(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{TwitchAnalyticsEventTypes.Subscriptions}') => @.value)
            ),
            {nameof(CategoryChartMetric.ChatCount)}: COUNT(FILTER(*.EventType => @ = '{TwitchAnalyticsEventTypes.ChatMessage}'))
        }}
        FROM analytics_events";

    public GetTopCategoriesQuery(AnalyticsFilter filter)
    {
        Filter = filter;
    }

    public string GetFilteredQuery()
    {
        return $@"{InternalQuery}
            WHERE (
                $.EventType = '{TwitchAnalyticsEventTypes.ViewerCount}' OR 
                $.EventType = '{TwitchAnalyticsEventTypes.Bits}' OR 
                $.EventType = '{TwitchAnalyticsEventTypes.Subscriptions}' OR 
                $.EventType = '{TwitchAnalyticsEventTypes.ChatMessage}'
            )
            AND {Filter.ToWhereClause()}
            GROUP BY $.Metadata.{TwitchAnalyticsMetaDataTypes.Category}";
    }

    public IEnumerable<CategoryChartMetric> Query(ILiteDatabase connection)
    {
        var query = GetFilteredQuery();
        using var reader = connection.Execute(query);
        return reader
            .ToArray()
            .Select(x => new CategoryChartMetric(
                x[nameof(CategoryChartMetric.Category)], 
                (int)x[nameof(CategoryChartMetric.Viewers)].AsDouble, 
                (int)x[nameof(CategoryChartMetric.ChatCount)].AsDouble, 
                (int)x[nameof(CategoryChartMetric.Bits)].AsDouble, 
                (int)x[nameof(CategoryChartMetric.Subs)].AsDouble));
    }
}