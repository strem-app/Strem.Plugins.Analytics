using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;

namespace Strem.Plugins.Analytics.Services.Repositories.Queries;

public class GetMetricsQuery : IBsonQuery
{
    public string SourceContext { get; }
    public string PlatformContext { get; }
    public DateTime StartPeriod { get; }
    public DateTime EndPeriod { get; }
    public IReadOnlyCollection<string> MetricTypes { get; }

    public GetMetricsQuery(string sourceContext, string platformContext, DateTime startPeriod, DateTime endPeriod, IReadOnlyCollection<string> metricTypes = null)
    {
        SourceContext = sourceContext;
        PlatformContext = platformContext;
        StartPeriod = startPeriod;
        EndPeriod = endPeriod;
        MetricTypes = metricTypes;
    }

    public IEnumerable<BsonDocument> Query(ILiteQueryable<BsonDocument> queryableDocuments)
    {
        var queryable = queryableDocuments
            .Where(x =>
                x[nameof(StreamMetric.PlatformContext)] == PlatformContext &&
                x[nameof(StreamMetric.SourceContext)] == SourceContext &&
                x[nameof(StreamMetric.MetricDateTime)] >= StartPeriod.ToString("O") &&
                x[nameof(StreamMetric.MetricDateTime)] <= EndPeriod.ToString("O"));
            
        if (MetricTypes is { Count: >= 0 })
        {
            queryable = queryable.Where(x =>
                MetricTypes.Contains(x[nameof(StreamMetric.MetricType)].AsString));
        }
        
        return queryable.ToEnumerable();
    }
}