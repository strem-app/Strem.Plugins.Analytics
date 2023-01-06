using Strem.Data;
using Strem.Plugins.Analytics.Models;

namespace Strem.Plugins.Analytics.Services.Repositories;

public interface IAnalyticsEventRepository : IRepository<AnalyticsEvent, Guid>
{
}