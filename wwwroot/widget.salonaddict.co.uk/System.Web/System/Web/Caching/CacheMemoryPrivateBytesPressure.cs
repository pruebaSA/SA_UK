namespace System.Web.Caching
{
    using System;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Hosting;

    internal sealed class CacheMemoryPrivateBytesPressure : CacheMemoryPressure
    {
        private long _memoryLimit;
        private long _pressureHighMemoryLimit;
        private DateTime _startupTime;
        private const long PRIVATE_BYTES_LIMIT_2GB = 0x32000000L;
        private const long PRIVATE_BYTES_LIMIT_3GB = 0x70800000L;
        private const long PRIVATE_BYTES_LIMIT_64BIT = 0x10000000000L;
        private static long s_autoPrivateBytesLimit = -1L;
        private static bool s_isIIS6 = HostingEnvironment.IsUnderIIS6Process;
        private static long s_lastReadPrivateBytes;
        private static DateTime s_lastTimeReadPrivateBytes = DateTime.MinValue;
        private static uint s_pid = 0;
        private static int s_pollInterval;

        internal CacheMemoryPrivateBytesPressure()
        {
            base._pressureHigh = 0x63;
            base._pressureMiddle = 0x62;
            base._pressureLow = 0x61;
            this._startupTime = DateTime.UtcNow;
            base.InitHistory();
        }

        protected override int GetCurrentPressure()
        {
            if (this._memoryLimit == 0L)
            {
                return 0;
            }
            long privateBytes = GetPrivateBytes(false);
            if (this._pressureHighMemoryLimit != 0L)
            {
                PerfCounters.SetCounter(AppPerfCounter.CACHE_PERCENT_PROC_MEM_LIMIT_USED, (int) (privateBytes >> 10));
            }
            return (int) ((privateBytes * 100L) / this._memoryLimit);
        }

        internal static long GetPrivateBytes(bool nocache)
        {
            long num;
            int num2 = 0;
            if (s_isIIS6)
            {
                long num3;
                num2 = UnsafeNativeMethods.GetPrivateBytesIIS6(out num3, nocache);
                num = num3;
            }
            else
            {
                uint num4;
                uint privatePageCount = 0;
                num2 = UnsafeNativeMethods.GetProcessMemoryInformation(s_pid, out privatePageCount, out num4, nocache);
                num = privatePageCount << 20;
            }
            if (num2 == 0)
            {
                s_lastReadPrivateBytes = num;
                s_lastTimeReadPrivateBytes = DateTime.UtcNow;
            }
            return num;
        }

        internal bool HasLimit() => 
            (this._memoryLimit != 0L);

        internal override void ReadConfig(CacheSection cacheSection)
        {
            long privateBytesLimit = cacheSection.PrivateBytesLimit;
            if (UnsafeNativeMethods.GetModuleHandle("aspnet_wp.exe") != IntPtr.Zero)
            {
                this._memoryLimit = UnsafeNativeMethods.PMGetMemoryLimitInMB() << 20;
            }
            else if (UnsafeNativeMethods.GetModuleHandle("w3wp.exe") != IntPtr.Zero)
            {
                IServerConfig instance = ServerConfig.GetInstance();
                this._memoryLimit = instance.GetW3WPMemoryLimitInKB() << 10;
            }
            if ((privateBytesLimit == 0L) && (this._memoryLimit == 0L))
            {
                this._memoryLimit = AutoPrivateBytesLimit;
            }
            else if ((privateBytesLimit != 0L) && (this._memoryLimit != 0L))
            {
                this._memoryLimit = Math.Min(this._memoryLimit, privateBytesLimit);
            }
            else if (privateBytesLimit != 0L)
            {
                this._memoryLimit = privateBytesLimit;
            }
            if (this._memoryLimit > 0L)
            {
                if (s_pid == 0)
                {
                    s_pid = (uint) SafeNativeMethods.GetCurrentProcessId();
                }
                if (this._memoryLimit >= 0x10000000L)
                {
                    base._pressureHigh = (int) Math.Max((long) 90L, (long) (((this._memoryLimit - 0x6000000L) * 100L) / this._memoryLimit));
                    base._pressureLow = (int) Math.Max((long) 80L, (long) (((this._memoryLimit - 0xe000000L) * 100L) / this._memoryLimit));
                    base._pressureMiddle = (base._pressureHigh + base._pressureLow) / 2;
                }
                else
                {
                    base._pressureHigh = 90;
                    base._pressureMiddle = 0x55;
                    base._pressureLow = 0x4e;
                }
                this._pressureHighMemoryLimit = (base._pressureHigh * this._memoryLimit) / 100L;
            }
            s_pollInterval = (int) Math.Min(cacheSection.PrivateBytesPollTime.TotalMilliseconds, 2147483647.0);
            PerfCounters.SetCounter(AppPerfCounter.CACHE_PERCENT_PROC_MEM_LIMIT_USED_BASE, (int) (this._pressureHighMemoryLimit >> 10));
        }

        internal static long AutoPrivateBytesLimit
        {
            get
            {
                if (s_autoPrivateBytesLimit == -1L)
                {
                    bool flag = IntPtr.Size == 8;
                    long totalPhysical = CacheMemoryPressure.TotalPhysical;
                    long totalVirtual = CacheMemoryPressure.TotalVirtual;
                    if (totalPhysical != 0L)
                    {
                        long num3;
                        if (flag)
                        {
                            num3 = 0x10000000000L;
                        }
                        else if (totalVirtual > 0x80000000L)
                        {
                            num3 = 0x70800000L;
                        }
                        else
                        {
                            num3 = 0x32000000L;
                        }
                        long num4 = HostingEnvironment.IsHosted ? ((totalPhysical * 3L) / 5L) : totalPhysical;
                        s_autoPrivateBytesLimit = Math.Min(num4, num3);
                    }
                    else
                    {
                        s_autoPrivateBytesLimit = flag ? 0x10000000000L : 0x32000000L;
                    }
                }
                return s_autoPrivateBytesLimit;
            }
        }

        internal long MemoryLimit =>
            this._memoryLimit;

        internal static int PollInterval =>
            s_pollInterval;

        internal long PressureHighMemoryLimit =>
            this._pressureHighMemoryLimit;
    }
}

