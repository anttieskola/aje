namespace AJE.Domain.Extensions;

public static class DateTimeExtensions
{
    public static string ToBestString(this DateTime dateTime) =>
        dateTime.ToString("yyyy-MM-dd HH:mm:ss");
}

public static class DateTimeOffsetExtensions
{
    public static string ToBestString(this DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss zzz");
}