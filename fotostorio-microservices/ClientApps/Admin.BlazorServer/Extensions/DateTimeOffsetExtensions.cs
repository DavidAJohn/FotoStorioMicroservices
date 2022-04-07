namespace Admin.BlazorServer.Extensions;

public static class DateTimeOffsetExtensions
{
    public static string ToUKStandardDate(this DateTimeOffset dateTime)
    {
        // returns date as dd/mm/yy (eg. 06/04/2022)
        return String.Format("{0:dd/MM/yyyy}", dateTime);
    }
}
