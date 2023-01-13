using Strem.Core.Extensions;
using Strem.Plugins.Analytics.Models.Filtering;

namespace Strem.Plugins.Analytics.Extensions;

public static class DateTimeExtensions
{
    public static DateTime RoundToNearest(this DateTime datetime, TimeUnitRounding rounding)
    {
        var timespan = rounding.TimeUnit.ToTimeSpan(rounding.UnitValue > 0 ? rounding.UnitValue : 1);
        return datetime.RoundToNearest(timespan);
    }
}