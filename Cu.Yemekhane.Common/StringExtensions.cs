using System.Globalization;
namespace Cu.Yemekhane.Common;

public static class StringExtensions
{
    public static bool IsParseableAsDate(this string value)
    {
        bool result = false;
        if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _))
            result = true;
        return result;
    }
}