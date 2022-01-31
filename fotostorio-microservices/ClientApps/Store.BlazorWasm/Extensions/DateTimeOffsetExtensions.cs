namespace Store.BlazorWasm.Extensions;

public static class DateTimeOffsetExtensions
{
    public static string ToUKStandardDate(this DateTimeOffset dateTime)
    {
        // returns date as dd/mm/yy (eg. 30/07/2021)
        return String.Format("{0:dd/MM/yyyy}", dateTime);
    }
}
