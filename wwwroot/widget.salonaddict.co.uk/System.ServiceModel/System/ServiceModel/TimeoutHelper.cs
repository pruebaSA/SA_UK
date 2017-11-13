namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.Threading;

    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeoutHelper
    {
        internal static TimeSpan MaxWait;
        private DateTime deadline;
        private bool deadlineSet;
        private TimeSpan originalTimeout;
        internal static TimeSpan DefaultBufferTime =>
            TimeSpan.FromMilliseconds(150.0);
        internal static TimeSpan Infinite =>
            TimeSpan.MaxValue;
        internal void SetDeadline()
        {
            this.deadline = DateTime.UtcNow + this.originalTimeout;
            this.deadlineSet = true;
        }

        internal TimeoutHelper(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", timeout, System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0")));
            }
            if (IsTooLarge(timeout))
            {
                timeout = MaxWait;
            }
            this.originalTimeout = timeout;
            this.deadline = DateTime.MaxValue;
            this.deadlineSet = timeout == TimeSpan.MaxValue;
        }

        public TimeSpan OriginalTimeout =>
            this.originalTimeout;
        public static bool IsTooLarge(TimeSpan timeout) => 
            ((timeout > MaxWait) && (timeout != Infinite));

        public TimeSpan MinRemainingTime(TimeSpan otherTimeout)
        {
            TimeSpan span = this.RemainingTime();
            if (span < otherTimeout)
            {
                return span;
            }
            return otherTimeout;
        }

        public TimeSpan MinRemainingTime(TimeoutHelper other)
        {
            if (!this.deadlineSet)
            {
                this.SetDeadline();
            }
            if (!other.deadlineSet)
            {
                other.SetDeadline();
            }
            if (this.deadline < other.deadline)
            {
                return this.RemainingTime();
            }
            return other.RemainingTime();
        }

        public TimeSpan RemainingTime() => 
            this.RemainingTimeExpireZero();

        public TimeSpan RemainingTimeExpireZero()
        {
            if (!this.deadlineSet)
            {
                this.SetDeadline();
                return this.originalTimeout;
            }
            if (this.deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            TimeSpan span = (TimeSpan) (this.deadline - DateTime.UtcNow);
            if (span <= TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            return span;
        }

        public TimeSpan RemainingTimeExpireNegative()
        {
            if (!this.deadlineSet)
            {
                this.SetDeadline();
                return this.originalTimeout;
            }
            if (this.deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            return (TimeSpan) (this.deadline - DateTime.UtcNow);
        }

        public TimeSpan ElapsedTime() => 
            (this.originalTimeout - this.RemainingTimeExpireZero());

        public static TimeSpan ReserveTimeAtEnd(TimeSpan timeout) => 
            ReserveTimeAtEnd(timeout, DefaultBufferTime);

        public static TimeSpan ReserveTimeAtEnd(TimeSpan timeout, TimeSpan bufferTime)
        {
            if (timeout < TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            if (timeout >= Add(bufferTime, bufferTime))
            {
                return Add(timeout, TimeSpan.Zero - bufferTime);
            }
            return Ticks.ToTimeSpan((Ticks.FromTimeSpan(timeout) / 2L) + 1L);
        }

        public static TimeSpan FromMilliseconds(int milliseconds)
        {
            if (milliseconds == -1)
            {
                return TimeSpan.MaxValue;
            }
            return TimeSpan.FromMilliseconds((double) milliseconds);
        }

        public static TimeSpan FromMilliseconds(uint milliseconds)
        {
            if (milliseconds == uint.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            return TimeSpan.FromMilliseconds((double) milliseconds);
        }

        public static int ToMilliseconds(TimeSpan timeout)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return -1;
            }
            long ticks = Ticks.FromTimeSpan(timeout);
            if ((ticks / 0x2710L) > 0x7fffffffL)
            {
                return 0x7fffffff;
            }
            return Ticks.ToMilliseconds(ticks);
        }

        public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2) => 
            Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));

        public static DateTime Add(DateTime time, TimeSpan timeout)
        {
            if ((timeout >= TimeSpan.Zero) && ((DateTime.MaxValue - time) <= timeout))
            {
                return DateTime.MaxValue;
            }
            if ((timeout <= TimeSpan.Zero) && ((DateTime.MinValue - time) >= timeout))
            {
                return DateTime.MinValue;
            }
            return (time + timeout);
        }

        public static DateTime Subtract(DateTime time, TimeSpan timeout) => 
            Add(time, TimeSpan.Zero - timeout);

        public static TimeSpan Divide(TimeSpan timeout, int factor)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            return Ticks.ToTimeSpan((Ticks.FromTimeSpan(timeout) / ((long) factor)) + 1L);
        }

        public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout, bool exitSync)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                waitHandle.WaitOne();
                return true;
            }
            if (IsTooLarge(timeout))
            {
                timeout = MaxWait;
            }
            return waitHandle.WaitOne(timeout, exitSync);
        }

        static TimeoutHelper()
        {
            MaxWait = TimeSpan.FromMilliseconds(2147483647.0);
        }
    }
}

