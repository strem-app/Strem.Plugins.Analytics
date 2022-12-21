using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Database;

namespace Strem.Plugins.Analytics.Services.Repositories;

public class StreamMetricRepository : Repository<StreamMetric, Guid>, IStreamMetricRepository
{
    public StreamMetricRepository(IAnalyticsDatabase connection) : base(connection, "stream_metric")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}