using System;

namespace SBMessenger
{
    public static class DateTimeConversion
    {
        public static DateTime UnixTimeToDateTime(long unixTime)
        {
            return UnixStartTime.AddSeconds(Convert.ToDouble(unixTime));
        }
        private static readonly DateTime UnixStartTime = new DateTime(
        1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    }
}
