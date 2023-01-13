using Strem.Core.Types;
using Strem.Plugins.Analytics.Models.Filtering;
using Strem.Plugins.Analytics.Viewer.Types;

namespace Strem.Plugins.Analytics.Viewer.Extensions;

public static class TimeUnitRoundingExtensions
{
    public static int ToSubStringAccuracy(this TimeUnitRounding rounding)
    {
        switch (rounding.TimeUnit)
        {
            case TimeUnitType.Seconds: return DateTimeStringAccuracy.SecondsAccuracy;
            case TimeUnitType.Hours: return DateTimeStringAccuracy.HourAccuracy;
            case TimeUnitType.Days: return DateTimeStringAccuracy.DayAccuracy;
            case TimeUnitType.Months: return DateTimeStringAccuracy.MonthAccuracy;
            case TimeUnitType.Years: return DateTimeStringAccuracy.YearAccuracy;
            default: return DateTimeStringAccuracy.MinuteAccuracy;
        }
    }
}