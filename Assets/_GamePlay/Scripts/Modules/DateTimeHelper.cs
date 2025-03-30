using System;

public static class DateTimeHelper
{
    public static DateTime? dateTimeNow;
    public static long dateAddTime;
    public static DateTime GetNow()
    {        
        return (dateTimeNow ?? DateTime.Now).AddSeconds(dateAddTime);
    }

    public static long GetTimeStampNow()
    {
        DateTime currentDateTime = GetNow();
        DateTimeOffset dto = new DateTimeOffset(currentDateTime);
        long unixTimestamp = dto.ToUnixTimeSeconds();
        return unixTimestamp;
    }

    public static DateTime TimeSharpToDateTime(long time)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        return dateTime;
    }
}
