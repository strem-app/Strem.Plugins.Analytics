using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Types;
using Strem.Plugins.Analytics.Viewer.Extensions;

namespace Strem.Plugins.Analytics.Viewer.Queries;

public class GetTopChattersQuery : IRawQuery<IEnumerable<KeyedMetric>>
{
    public AnalyticsFilter Filter { get; }
    
    private static string InternalQuery = $@"SELECT {{
            {nameof(KeyedMetric.Key)}: @key, 
            {nameof(KeyedMetric.Value)}: COUNT(*._id)
        }}
        FROM analytics_events";

    public GetTopChattersQuery(AnalyticsFilter filter)
    {
        Filter = filter;
    }

    public string GetFilteredQuery()
    {
        return $@"{InternalQuery}
            WHERE $.EventType = '{AnalyticsEventTypes.ChatMessage}'
            AND {Filter.ToWhereClause()}
            GROUP BY $.UserContext";
    }

    public IEnumerable<KeyedMetric> Query(ILiteDatabase connection)
    {
        var query = GetFilteredQuery();
        using var reader = connection.Execute(query);
        return reader
            .ToArray()
            .Select(x => new KeyedMetric(x[nameof(KeyedMetric.Key)], x[nameof(KeyedMetric.Value)].AsInt32));
    }
}