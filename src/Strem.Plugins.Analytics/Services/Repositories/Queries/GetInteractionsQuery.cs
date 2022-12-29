using LiteDB;
using Strem.Data;
using Strem.Plugins.Analytics.Models;

namespace Strem.Plugins.Analytics.Services.Repositories.Queries;

public class GetInteractionsQuery : IBsonQuery
{
    public string SourceContext { get; }
    public string PlatformContext { get; }
    public DateTime StartPeriod { get; }
    public DateTime EndPeriod { get; }
    public IReadOnlyCollection<string> InteractionTypes { get; }

    public GetInteractionsQuery(string sourceContext, string platformContext, DateTime startPeriod, DateTime endPeriod, IReadOnlyCollection<string> interactionTypes)
    {
        SourceContext = sourceContext;
        PlatformContext = platformContext;
        StartPeriod = startPeriod;
        EndPeriod = endPeriod;
        InteractionTypes = interactionTypes;
    }

    public IEnumerable<BsonDocument> Query(ILiteQueryable<BsonDocument> queryableDocuments)
    {
        return queryableDocuments
            .Where(x =>
                x[nameof(StreamInteraction.PlatformContext)] == PlatformContext &&
                x[nameof(StreamInteraction.SourceContext)] == SourceContext &&
                InteractionTypes.Contains(x[nameof(StreamInteraction.InteractionType)].AsString) &&
                x[nameof(StreamInteraction.InteractionDateTime)] >= StartPeriod.ToString("O") &&
                x[nameof(StreamInteraction.InteractionDateTime)] <= EndPeriod.ToString("O"))
            .ToEnumerable();
    }
}