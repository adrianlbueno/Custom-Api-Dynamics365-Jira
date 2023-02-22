using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static CultureInfo DefaultCulture { get; set; } = new CultureInfo("de-de");

        public static int GetWeekOfYear(this DateTime datetime, CultureInfo culture = null)
        {
            return (culture = culture ?? DefaultCulture)?.Calendar.GetWeekOfYear(datetime, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek) ?? -1;
        }

        public static int GetWeekOfYear(this DateTime? datetime, CultureInfo culture = null)
        {
            return datetime.HasValue ? datetime.Value.GetWeekOfYear(culture) : -1;
        }
    }
}
