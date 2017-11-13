namespace System.ServiceModel
{
    using System;

    internal static class TimeSpanHelper
    {
        public static TimeSpan FromDays(int days, string text) => 
            TimeSpan.FromTicks(0xc92a69c000L * days);

        public static TimeSpan FromMilliseconds(int ms, string text) => 
            TimeSpan.FromTicks(0x2710L * ms);

        public static TimeSpan FromMinutes(int minutes, string text) => 
            TimeSpan.FromTicks(0x23c34600L * minutes);

        public static TimeSpan FromSeconds(int seconds, string text) => 
            TimeSpan.FromTicks(0x989680L * seconds);
    }
}

