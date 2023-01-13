using System.Text;
using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Types;
using Strem.Plugins.Analytics.Viewer.Extensions;
using Strem.Plugins.Analytics.Viewer.Models;

namespace Strem.Plugins.Analytics.Viewer.Queries;

public class GetUserInteractionQuery : IRawQuery<IEnumerable<InteractionMetric>>
{
    public AnalyticsFilter Filter { get; }
    
    private static string InternalQuery = $@"SELECT {{
            {nameof(InteractionMetric.Date)}: @key, 
            {nameof(InteractionMetric.ChatCount)}: COUNT(FILTER(*.EventType => @ = '{AnalyticsEventTypes.ChatMessage}')),
            {nameof(InteractionMetric.UsersJoined)}: COUNT(FILTER(*.EventType => @ = '{AnalyticsEventTypes.UserJoined}')),
            {nameof(InteractionMetric.UsersLeft)}: COUNT(FILTER(*.EventType => @ = '{AnalyticsEventTypes.UserLeft}')),
            {nameof(InteractionMetric.Viewers)}: AVG(
                MAP(FILTER(MAP(* => {{type: @.EventType, value: @.EventValue}}) => @.type = '{AnalyticsEventTypes.ViewerCount}') => @.value)
            )    
        }}
        FROM analytics_events";

    public GetUserInteractionQuery(AnalyticsFilter filter)
    {
        Filter = filter;
    }

    public string GetFilteredQuery()
    {
        return $@"{InternalQuery}
            WHERE ($.EventType = '{AnalyticsEventTypes.ChatMessage}' 
            OR $.EventType = '{AnalyticsEventTypes.UserJoined}' 
            OR $.EventType = '{AnalyticsEventTypes.UserLeft}'
            OR $.EventType = '{AnalyticsEventTypes.ViewerCount}')
            AND {Filter.ToWhereClause()}
            GROUP BY SUBSTRING($.EventDateTime, 0, {Filter.TimeUnitRounding.ToSubStringAccuracy()})";
    }

    public IEnumerable<InteractionMetric> Query(ILiteDatabase connection)
    {
        var query = GetFilteredQuery();
        using var reader = connection.Execute(query);
        return reader
            .ToArray()
            .Select(x => new InteractionMetric(
                DateTime.Parse(x[nameof(InteractionMetric.Date)]), 
                (int)x[nameof(InteractionMetric.Viewers)].AsDouble, x[nameof(InteractionMetric.ChatCount)], 
                x[nameof(InteractionMetric.UsersJoined)], x[nameof(InteractionMetric.UsersLeft)]));
    }
}