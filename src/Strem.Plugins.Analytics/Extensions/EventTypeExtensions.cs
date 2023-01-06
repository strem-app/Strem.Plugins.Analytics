namespace Strem.Plugins.Analytics.Extensions;

public static class EventTypeExtensions
{
    public static string GetNiceTypeName(this string eventType)
    { return eventType.Replace("-", " ").Replace("_", " "); }
}