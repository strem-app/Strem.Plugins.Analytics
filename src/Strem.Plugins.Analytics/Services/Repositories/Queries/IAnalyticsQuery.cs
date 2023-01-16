using LiteDB;

namespace Strem.Plugins.Analytics.Services.Repositories.Queries;

public interface IAnalyticsQuery<out T>
{
    IEnumerable<T> Query(ILiteQueryable<BsonDocument> queryableDocuments);
}