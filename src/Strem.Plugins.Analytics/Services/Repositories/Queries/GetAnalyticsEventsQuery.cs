using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Services.Repositories.Queries;

public class GetAnalyticsEventsQuery : IBsonQuery
{
    public AnalyticsFilter Filter { get; }
    public IReadOnlyCollection<string> EventTypes { get; }

    public GetAnalyticsEventsQuery(AnalyticsFilter filter, IReadOnlyCollection<string> eventTypes = null)
    {
        Filter = filter;
        EventTypes = eventTypes;
    }

    public IEnumerable<BsonDocument> Query(ILiteQueryable<BsonDocument> queryableDocuments)
    {
        var queryable = queryableDocuments
            .Where(x =>
                x[nameof(AnalyticsEvent.PlatformContext)] == Filter.PlatformContext &&
                x[nameof(AnalyticsEvent.SourceContext)] == Filter.SourceContext &&
                x[nameof(AnalyticsEvent.EventDateTime)] >= Filter.StartPeriod.ToString("O") &&
                x[nameof(AnalyticsEvent.EventDateTime)] <= Filter.EndPeriod.ToString("O"));
            
        if (EventTypes is { Count: >= 0 })
        { queryable = queryable.Where(x => EventTypes.Contains(x[nameof(AnalyticsEvent.EventType)].AsString)); }
        
        return queryable.ToEnumerable();
    }
}