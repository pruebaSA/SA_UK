namespace System.Web.Caching
{
    using System;
    using System.Web;
    using System.Web.Configuration;

    internal abstract class CacheMemoryPressure
    {
        protected int _i0;
        protected int _pressureAvg;
        protected int _pressureHigh;
        protected int[] _pressureHist;
        protected int _pressureLow;
        protected int _pressureMiddle;
        protected int _pressureTotal;
        protected const long GIGABYTE = 0x40000000L;
        protected const int GIGABYTE_SHIFT = 30;
        protected const int HISTORY_COUNT = 6;
        protected const long KILOBYTE = 0x400L;
        protected const int KILOBYTE_SHIFT = 10;
        protected const long MEGABYTE = 0x100000L;
        protected const int MEGABYTE_SHIFT = 20;
        private static long s_totalPhysical;
        private static long s_totalVirtual;
        protected const long TERABYTE = 0x10000000000L;
        protected const int TERABYTE_SHIFT = 40;

        protected CacheMemoryPressure()
        {
        }

        protected abstract int GetCurrentPressure();
        internal static void GetMemoryStatusOnce()
        {
            UnsafeNativeMethods.MEMORYSTATUSEX memoryStatusEx = new UnsafeNativeMethods.MEMORYSTATUSEX();
            memoryStatusEx.Init();
            if (UnsafeNativeMethods.GlobalMemoryStatusEx(ref memoryStatusEx) != 0)
            {
                s_totalPhysical = memoryStatusEx.ullTotalPhys;
                s_totalVirtual = memoryStatusEx.ullTotalVirtual;
            }
        }

        protected void InitHistory()
        {
            int currentPressure = this.GetCurrentPressure();
            this._pressureHist = new int[6];
            for (int i = 0; i < 6; i++)
            {
                this._pressureHist[i] = currentPressure;
                this._pressureTotal += currentPressure;
            }
            this._pressureAvg = currentPressure;
        }

        internal bool IsAboveHighPressure() => 
            (this.PressureLast >= this.PressureHigh);

        internal bool IsAboveMediumPressure() => 
            (this.PressureLast > this.PressureMiddle);

        internal virtual void ReadConfig(CacheSection cacheSection)
        {
        }

        internal void Update()
        {
            int currentPressure = this.GetCurrentPressure();
            this._i0 = (this._i0 + 1) % 6;
            this._pressureTotal -= this._pressureHist[this._i0];
            this._pressureTotal += currentPressure;
            this._pressureHist[this._i0] = currentPressure;
            this._pressureAvg = this._pressureTotal / 6;
        }

        internal int PressureAvg =>
            this._pressureAvg;

        internal int PressureHigh =>
            this._pressureHigh;

        internal int PressureLast =>
            this._pressureHist[this._i0];

        internal int PressureLow =>
            this._pressureLow;

        internal int PressureMiddle =>
            this._pressureMiddle;

        internal static long TotalPhysical =>
            s_totalPhysical;

        internal static long TotalVirtual =>
            s_totalVirtual;
    }
}

