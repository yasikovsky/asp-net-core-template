using System;

namespace ProjectNameApi.Extensions
{
    public static class DateTimeExt
    {
        // ReSharper disable once InconsistentNaming
        public static string ToISODate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        // ReSharper disable once InconsistentNaming
        public static string ToISODateTime(this DateTime dt, bool fileSystemSafe = false)
        {
            if (fileSystemSafe)
                return dt.ToString("yyyy-MM-dd-HH-mm");

            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        // ReSharper disable once InconsistentNaming
        public static string ToISODateTimeFull(this DateTime dt, bool fileSystemSafe = false)
        {
            if (fileSystemSafe)
                return dt.ToString("yyyy-MM-dd-HH-mm-ss");

            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}