using System.Linq.Expressions;
using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Database;
using Strem.Plugins.Analytics.Services.Repositories.Queries;

namespace Strem.Plugins.Analytics.Services.Repositories;

public class AnalyticsEventRepository : Repository<AnalyticsEvent, Guid>, IAnalyticsEventRepository
{
    public AnalyticsEventRepository(IAnalyticsDatabase connection) : base(connection, "analytics_events")
    {
        Collection.EnsureIndex(nameof(AnalyticsEvent.EventDateTime));
    }

    public override BsonValue GetId(Guid id) => new(id);
}