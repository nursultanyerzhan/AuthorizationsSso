namespace AuthorizationsSso.Helpers;

public class TimeHelper
{
    public static long GetTimeStamp(DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }
}