namespace AXDD.BuildingBlocks.Common.Extensions;

/// <summary>
/// DateTime extension methods
/// </summary>
public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }

    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
    {
        return date >= start && date <= end;
    }
}
