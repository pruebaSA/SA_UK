namespace System.Web.Caching
{
    using System;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;

    internal class CacheCommon
    {
        internal CacheInternal _cacheInternal;
        protected internal CacheMemoryStats _cacheMemoryStats = new CacheMemoryStats();
        internal Cache _cachePublic = new Cache(0);
        internal int _currentPollInterval = 0x7530;
        internal bool _enableExpiration = true;
        internal bool _enableMemoryCollection = true;
        private int _gcCollectCount;
        internal int _inMemoryStatsUpdate;
        internal bool _internalConfigRead;
        internal Timer _timerMemoryStats;
        internal object _timerMemoryStatsLock = new object();
        internal DateTime _timerSuspendTime = DateTime.MinValue;
        private const int GC_BACKOFF_INTERVAL = 60;
        private const int GC_INTERVAL = 5;
        private const int MEMORYSTATUS_INTERVAL_30_SECONDS = 0x7530;
        private const int MEMORYSTATUS_INTERVAL_5_SECONDS = 0x1388;

        internal CacheCommon()
        {
        }

        private void AdjustTimer()
        {
            lock (this._timerMemoryStatsLock)
            {
                if (this._timerMemoryStats != null)
                {
                    if (this._cacheMemoryStats.IsAboveHighPressure())
                    {
                        if (this._currentPollInterval > 0x1388)
                        {
                            this._currentPollInterval = 0x1388;
                            this._timerMemoryStats.Change(this._currentPollInterval, this._currentPollInterval);
                        }
                    }
                    else if ((this._cacheMemoryStats.PrivateBytesPressure.PressureLast > (this._cacheMemoryStats.PrivateBytesPressure.PressureLow / 2)) || (this._cacheMemoryStats.TotalMemoryPressure.PressureLast > (this._cacheMemoryStats.TotalMemoryPressure.PressureLow / 2)))
                    {
                        int num = Math.Min(CacheMemoryPrivateBytesPressure.PollInterval, 0x7530);
                        if (this._currentPollInterval != num)
                        {
                            this._currentPollInterval = num;
                            this._timerMemoryStats.Change(this._currentPollInterval, this._currentPollInterval);
                        }
                    }
                    else if (this._currentPollInterval != CacheMemoryPrivateBytesPressure.PollInterval)
                    {
                        this._currentPollInterval = CacheMemoryPrivateBytesPressure.PollInterval;
                        this._timerMemoryStats.Change(this._currentPollInterval, this._currentPollInterval);
                    }
                }
            }
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.EnableCacheMemoryTimer(false);
            }
        }

        internal void EnableCacheMemoryTimer(bool enable)
        {
            lock (this._timerMemoryStatsLock)
            {
                if (enable)
                {
                    if (this._timerMemoryStats == null)
                    {
                        this._timerMemoryStats = new Timer(new TimerCallback(this.MemoryStatusTimerCallback), null, this._currentPollInterval, this._currentPollInterval);
                    }
                    else
                    {
                        this._timerMemoryStats.Change(this._currentPollInterval, this._currentPollInterval);
                    }
                }
                else
                {
                    Timer comparand = this._timerMemoryStats;
                    if ((comparand != null) && (Interlocked.CompareExchange<Timer>(ref this._timerMemoryStats, null, comparand) == comparand))
                    {
                        comparand.Dispose();
                    }
                }
            }
            if (!enable)
            {
                while (this._inMemoryStatsUpdate != 0)
                {
                    Thread.Sleep(100);
                }
            }
        }

        internal void GcCollect()
        {
            bool flag2;
            int num = CacheMemoryPrivateBytesPressure.PollInterval / 0x3e8;
            int num2 = Math.Max(Math.Min(num, 60), 5);
            bool flag = this._cacheMemoryStats.TotalMemoryPressure.IsAboveHighPressure();
            long totalMemoryChange = 0L;
            UnsafeNativeMethods.SetGCLastCalledTime(out flag2, flag ? num2 : 5);
            if (flag2)
            {
                long privateBytes = CacheMemoryPrivateBytesPressure.GetPrivateBytes(true);
                long totalMemory = GC.GetTotalMemory(false);
                GC.Collect();
                this._gcCollectCount++;
                long num6 = CacheMemoryPrivateBytesPressure.GetPrivateBytes(true);
                long num7 = GC.GetTotalMemory(false);
                totalMemoryChange = Math.Max((long) (privateBytes - num6), (long) (totalMemory - num7));
            }
            if ((!flag2 || flag) || this._cacheMemoryStats.IsGcCollectIneffective(totalMemoryChange))
            {
                this._timerSuspendTime = DateTime.UtcNow.AddSeconds((double) num2);
            }
        }

        private void MemoryStatusTimerCallback(object state)
        {
            if (Interlocked.Exchange(ref this._inMemoryStatsUpdate, 1) == 0)
            {
                try
                {
                    if (DateTime.UtcNow >= this._timerSuspendTime)
                    {
                        this._cacheMemoryStats.Update();
                        this.AdjustTimer();
                        bool flag = this._cacheInternal.ReviewMemoryStats();
                        if (this._cacheMemoryStats.IsAboveHighPressure() && flag)
                        {
                            this.GcCollect();
                        }
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref this._inMemoryStatsUpdate, 0);
                }
            }
        }

        internal void ReadCacheInternalConfig(CacheSection cacheSection)
        {
            if (!this._internalConfigRead)
            {
                lock (this)
                {
                    if (!this._internalConfigRead)
                    {
                        this._internalConfigRead = true;
                        if (cacheSection != null)
                        {
                            this._enableMemoryCollection = !cacheSection.DisableMemoryCollection;
                            this._enableExpiration = !cacheSection.DisableExpiration;
                            this._cacheMemoryStats.ReadConfig(cacheSection);
                            this._currentPollInterval = CacheMemoryPrivateBytesPressure.PollInterval;
                            this.ResetFromConfigSettings();
                        }
                    }
                }
            }
        }

        internal void ResetFromConfigSettings()
        {
            this.EnableCacheMemoryTimer(this._enableMemoryCollection);
            this._cacheInternal.EnableExpirationTimer(this._enableExpiration);
        }

        internal void SetCacheInternal(CacheInternal cacheInternal)
        {
            this._cacheInternal = cacheInternal;
            this._cachePublic.SetCacheInternal(cacheInternal);
        }
    }
}

