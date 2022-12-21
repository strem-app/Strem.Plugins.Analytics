using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Database;

namespace Strem.Plugins.Analytics.Services.Repositories;

public class StreamInteractionRepository : Repository<StreamInteraction, Guid>, IStreamInteractionRepository
{
    public StreamInteractionRepository(IAnalyticsDatabase connection) : base(connection, "stream_interactions")
    {}

    public override BsonValue GetId(Guid id) => new(id);
}