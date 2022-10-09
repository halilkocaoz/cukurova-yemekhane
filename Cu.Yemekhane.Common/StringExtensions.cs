using System.Globalization;

namespace Cu.Yemekhane.Common;

public static class StringExtensions
{
    public static bool ParseableAsDate(this string value)
    {
        return DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out _);
    }
}