namespace System.ServiceModel.Channels
{
    using System;
    using System.Security;

    internal static class Ticks
    {
        public static long Add(long firstTicks, long secondTicks)
        {
            if ((firstTicks == 0x7fffffffffffffffL) || (firstTicks == -9223372036854775808L))
            {
                return firstTicks;
            }
            if ((secondTicks == 0x7fffffffffffffffL) || (secondTicks == -9223372036854775808L))
            {
                return secondTicks;
            }
            if ((firstTicks >= 0L) && ((0x7fffffffffffffffL - firstTicks) <= secondTicks))
            {
                return 0x7ffffffffffffffeL;
            }
            if ((firstTicks <= 0L) && ((-9223372036854775808L - firstTicks) >= secondTicks))
            {
                return -9223372036854775807L;
            }
            return (firstTicks + secondTicks);
        }

        public static long FromMilliseconds(int milliseconds) => 
            (milliseconds * 0x2710L);

        public static long FromTimeSpan(TimeSpan duration) => 
            duration.Ticks;

        public static int ToMilliseconds(long ticks) => 
            ((int) (ticks / 0x2710L));

        public static TimeSpan ToTimeSpan(long ticks) => 
            new TimeSpan(ticks);

        public static long Now
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                long num;
                SafeNativeMethods.GetSystemTimeAsFileTime(out num);
                return num;
            }
        }
    }
}

