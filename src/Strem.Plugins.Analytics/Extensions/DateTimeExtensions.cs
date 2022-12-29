namespace Strem.Plugins.Analytics.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetStartOfMonth(this DateTime date)
    { return new DateTime(date.Year, date.Month, 1).Date; }

    public static DateTime GetEndOfMonth(this DateTime date)
    { return date.GetStartOfMonth().AddMonths(1).AddMilliseconds(-1); }
}