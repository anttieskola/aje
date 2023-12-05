namespace AJE.Domain;

public enum TimePeriod
{
    Hour = 1,
    Day = 3,
    Week = 5,
    Month = 6
}

public static class DatetimeOffSetExtensions
{
    /// <summary>
    /// Add a time period to a DateTimeOffset
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="timePeriod"></param>
    /// <returns></returns>
    /// <exception cref="PlatformException"></exception>
    public static DateTimeOffset AddPeriod(this DateTimeOffset dateTimeOffset, TimePeriod timePeriod)
        => timePeriod switch
        {
            TimePeriod.Hour => dateTimeOffset.AddHours(1),
            TimePeriod.Day => dateTimeOffset.AddDays(1),
            TimePeriod.Week => dateTimeOffset.AddDays(7),
            TimePeriod.Month => dateTimeOffset.AddMonths(1),
            _ => throw new PlatformException($"invalid time period {timePeriod}")
        };
}