using Strem.Data;
using Strem.Plugins.Analytics.Models;
using Strem.Plugins.Analytics.Services.Repositories.Queries;

namespace Strem.Plugins.Analytics.Services.Repositories;

public interface IAnalyticsEventRepository : IRepository<AnalyticsEvent, Guid>
{
}