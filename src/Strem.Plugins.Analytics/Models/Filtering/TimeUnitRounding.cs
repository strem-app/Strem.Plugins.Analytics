using Strem.Core.Types;

namespace Strem.Plugins.Analytics.Models.Filtering;

public class TimeUnitRounding
{
    public TimeUnitType TimeUnit { get; set; }
    public int UnitValue { get; set; }

    public TimeUnitRounding(TimeUnitType timeUnit = TimeUnitType.Minutes, int unitValue = 30)
    {
        TimeUnit = timeUnit;
        UnitValue = unitValue;
    }
}