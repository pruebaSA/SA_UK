namespace System.Web.Caching
{
    using System;
    using System.Web.Configuration;

    internal class CacheMemoryStats
    {
        private long _lastTotalMemoryChange;
        private long _minTotalMemoryChange = -1L;
        private CacheMemoryPrivateBytesPressure _pressurePrivateBytes;
        private CacheMemoryTotalMemoryPressure _pressureTotalMemory;

        internal CacheMemoryStats()
        {
            CacheMemoryPressure.GetMemoryStatusOnce();
            this._pressureTotalMemory = new CacheMemoryTotalMemoryPressure();
            this._pressurePrivateBytes = new CacheMemoryPrivateBytesPressure();
        }

        internal bool IsAboveHighPressure()
        {
            if (!this._pressureTotalMemory.IsAboveHighPressure())
            {
                return this._pressurePrivateBytes.IsAboveHighPressure();
            }
            return true;
        }

        internal bool IsAboveMediumPressure()
        {
            if (!this._pressureTotalMemory.IsAboveMediumPressure())
            {
                return this._pressurePrivateBytes.IsAboveMediumPressure();
            }
            return true;
        }

        internal bool IsGcCollectIneffective(long totalMemoryChange)
        {
            if ((this._minTotalMemoryChange == -1L) && this._pressurePrivateBytes.HasLimit())
            {
                this._minTotalMemoryChange = this._pressurePrivateBytes.MemoryLimit / 100L;
            }
            this._lastTotalMemoryChange = totalMemoryChange;
            return (totalMemoryChange < this._minTotalMemoryChange);
        }

        internal void ReadConfig(CacheSection cacheSection)
        {
            this._pressureTotalMemory.ReadConfig(cacheSection);
            this._pressurePrivateBytes.ReadConfig(cacheSection);
        }

        internal void Update()
        {
            this._pressureTotalMemory.Update();
            this._pressurePrivateBytes.Update();
        }

        internal CacheMemoryPrivateBytesPressure PrivateBytesPressure =>
            this._pressurePrivateBytes;

        internal CacheMemoryTotalMemoryPressure TotalMemoryPressure =>
            this._pressureTotalMemory;
    }
}

