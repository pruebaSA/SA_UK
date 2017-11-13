namespace System.Web.Caching
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Web;

    internal sealed class CacheSingle : CacheInternal
    {
        private CacheMultiple _cacheMultiple;
        private int _disposed;
        private Hashtable _entries;
        private CacheExpires _expires;
        private bool _flushUndercount;
        private ManualResetEvent _insertBlock;
        private int _insertBlockCalls;
        private int _iSubCache;
        private object _lock;
        private int _maxCount;
        private int _maxCountOverload;
        private int _publicCount;
        private int _totalCount;
        private CacheUsage _usage;
        private bool _useInsertBlock;
        private DateTime _utcLastTrimCompleted;
        private static readonly TimeSpan FLUSH_UNDERCOUNT_WAIT = new TimeSpan(0, 0, 1);
        private static readonly TimeSpan INSERT_BLOCK_WAIT = new TimeSpan(0, 0, 10);
        private const int MAX_COUNT = 0x3fffffff;
        private const int MAX_OVERLOAD_COUNT = 50;
        private const int MIN_COUNT = 10;

        internal CacheSingle(CacheCommon cacheCommon, CacheMultiple cacheMultiple, int iSubCache) : base(cacheCommon)
        {
            this._cacheMultiple = cacheMultiple;
            this._iSubCache = iSubCache;
            this._entries = new Hashtable(CacheKeyComparer.GetInstance());
            this._expires = new CacheExpires(this);
            this._usage = new CacheUsage(this);
            this._lock = new object();
            this._maxCount = 0x3fffffff;
            this._maxCountOverload = 0x40000031;
            this._insertBlock = new ManualResetEvent(true);
        }

        internal void BlockInsertIfNeeded()
        {
            if (base._cacheCommon._cacheMemoryStats.IsAboveHighPressure())
            {
                this._useInsertBlock = true;
                this.ResetInsertBlock();
            }
        }

        private int CalcMaxCount(CacheMemoryPressure pressure)
        {
            int pressureLast = pressure.PressureLast;
            int pressureAvg = pressure.PressureAvg;
            int pressureHigh = pressure.PressureHigh;
            int pressureLow = pressure.PressureLow;
            int pressureMiddle = pressure.PressureMiddle;
            int num6 = this._totalCount;
            int num7 = -1;
            if (pressureLast > pressureMiddle)
            {
                if (pressureLast >= pressureHigh)
                {
                    return (int) (((long) num6) / 2L);
                }
                num7 = (int) ((num6 * ((2 * pressureMiddle) - pressureLast)) / ((long) pressureMiddle));
                return Math.Max((int) ((0x13L * num6) / 20L), num7);
            }
            if (pressureLast == pressureMiddle)
            {
                if (this._maxCount < 0x3fffffff)
                {
                    return this._maxCount;
                }
                return num6;
            }
            if (pressureLast >= pressureLow)
            {
                if (pressureAvg < pressureMiddle)
                {
                    num7 = (int) ((num6 * ((2 * pressureMiddle) - pressureLast)) / ((long) pressureMiddle));
                }
                else
                {
                    num7 = num6;
                }
                if (this._maxCount < 0x3fffffff)
                {
                    num7 = Math.Max(num7, this._maxCount);
                }
                return num7;
            }
            return 0x3fffffff;
        }

        internal override IDictionaryEnumerator CreateEnumerator()
        {
            Hashtable hashtable = new Hashtable(this._publicCount);
            DateTime utcNow = DateTime.UtcNow;
            lock (this._lock)
            {
                foreach (DictionaryEntry entry in this._entries)
                {
                    CacheEntry entry2 = (CacheEntry) entry.Value;
                    if ((entry2.IsPublic && (entry2.State == CacheEntry.EntryState.AddedToCache)) && (!base._cacheCommon._enableExpiration || (utcNow <= entry2.UtcExpires)))
                    {
                        hashtable[entry2.Key] = entry2.Value;
                    }
                }
            }
            return hashtable.GetEnumerator();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (Interlocked.Exchange(ref this._disposed, 1) == 0))
            {
                if (this._expires != null)
                {
                    this._expires.EnableExpirationTimer(false);
                }
                CacheEntry[] entryArray = null;
                lock (this._lock)
                {
                    entryArray = new CacheEntry[this._entries.Count];
                    int num = 0;
                    foreach (DictionaryEntry entry in this._entries)
                    {
                        entryArray[num++] = (CacheEntry) entry.Value;
                    }
                }
                foreach (CacheEntry entry2 in entryArray)
                {
                    base.Remove(entry2, CacheItemRemovedReason.Removed);
                }
                this._insertBlock.Set();
                this.ReleaseInsertBlock();
            }
            base.Dispose(disposing);
        }

        internal override void EnableExpirationTimer(bool enable)
        {
            if (this._expires != null)
            {
                this._expires.EnableExpirationTimer(enable);
            }
        }

        private bool NeedsTrim()
        {
            bool flag = false;
            if (!base._cacheCommon._enableMemoryCollection || ((this._totalCount <= this._maxCountOverload) && ((this._totalCount <= this._maxCount) || !base._cacheCommon._cacheMemoryStats.IsAboveMediumPressure())))
            {
                return flag;
            }
            return (!this._flushUndercount || ((DateTime.UtcNow - this._utcLastTrimCompleted) >= FLUSH_UNDERCOUNT_WAIT));
        }

        private void ReleaseInsertBlock()
        {
            if (Interlocked.Decrement(ref this._insertBlockCalls) < 0)
            {
                ManualResetEvent event2 = this._insertBlock;
                this._insertBlock = null;
                event2.Close();
            }
        }

        private void ResetInsertBlock()
        {
            ManualResetEvent event2 = null;
            try
            {
                event2 = this.UseInsertBlock();
                if (event2 != null)
                {
                    event2.Reset();
                }
            }
            finally
            {
                if (event2 != null)
                {
                    this.ReleaseInsertBlock();
                }
            }
        }

        internal override bool ReviewMemoryStats()
        {
            int num = this.CalcMaxCount(base._cacheCommon._cacheMemoryStats.TotalMemoryPressure);
            if (base._cacheCommon._cacheMemoryStats.PrivateBytesPressure.HasLimit())
            {
                num = Math.Min(num, this.CalcMaxCount(base._cacheCommon._cacheMemoryStats.PrivateBytesPressure));
            }
            num = Math.Min(Math.Max(num, 10), 0x3fffffff);
            this._maxCount = num;
            this._maxCountOverload = this._maxCount + 50;
            return this.TrimIfNeeded();
        }

        private void SetInsertBlock()
        {
            ManualResetEvent event2 = null;
            try
            {
                event2 = this.UseInsertBlock();
                if (event2 != null)
                {
                    event2.Set();
                }
            }
            finally
            {
                if (event2 != null)
                {
                    this.ReleaseInsertBlock();
                }
            }
        }

        private void Trim()
        {
            int publicEntriesFlushed = 0;
            int delta = 0;
            try
            {
                do
                {
                    int num3 = this._totalCount - this._maxCount;
                    int num4 = 0;
                    if (num3 > 0)
                    {
                        num4 = this._expires.FlushExpiredItems(true);
                        int num5 = 0;
                        if (num4 < num3)
                        {
                            num5 = this._usage.FlushUnderUsedItems(num3 - num4, ref publicEntriesFlushed);
                            num4 += num5;
                            delta += num5;
                        }
                    }
                    this._utcLastTrimCompleted = DateTime.UtcNow;
                    this._flushUndercount = num4 < num3;
                }
                while (this.NeedsTrim());
                PerfCounters.IncrementCounterEx(AppPerfCounter.CACHE_TOTAL_TRIMS, delta);
                PerfCounters.IncrementCounterEx(AppPerfCounter.CACHE_API_TRIMS, publicEntriesFlushed);
                PerfCounters.IncrementCounterEx(AppPerfCounter.CACHE_OUTPUT_TRIMS, delta - publicEntriesFlushed);
            }
            catch
            {
            }
        }

        private bool TrimIfNeeded()
        {
            if (this.NeedsTrim())
            {
                this.Trim();
                return true;
            }
            return false;
        }

        internal void UnblockInsert()
        {
            if (this._useInsertBlock)
            {
                this._useInsertBlock = false;
                this.SetInsertBlock();
            }
        }

        internal override CacheEntry UpdateCache(CacheKey cacheKey, CacheEntry newEntry, bool replace, CacheItemRemovedReason removedReason, out object valueOld)
        {
            CacheEntry cacheEntry = null;
            CacheEntry key = null;
            CacheDependency dependency = null;
            bool flag4 = false;
            bool flag5 = false;
            DateTime minValue = DateTime.MinValue;
            CacheEntry.EntryState notInCache = CacheEntry.EntryState.NotInCache;
            bool flag6 = false;
            CacheItemRemovedReason removed = CacheItemRemovedReason.Removed;
            valueOld = null;
            bool flag2 = !replace && (newEntry == null);
            bool flag3 = !replace && (newEntry != null);
        Label_003E:
            if (flag4)
            {
                this.UpdateCache(cacheKey, null, true, CacheItemRemovedReason.Expired, out valueOld);
                flag4 = false;
            }
            cacheEntry = null;
            DateTime utcNow = DateTime.UtcNow;
            if ((this._useInsertBlock && (newEntry != null)) && newEntry.HasUsage())
            {
                this.WaitInsertBlock();
            }
            bool flag7 = false;
            if (!flag2)
            {
                try
                {
                }
                finally
                {
                    Monitor.Enter(this._lock);
                    flag7 = true;
                }
            }
            try
            {
                cacheEntry = (CacheEntry) this._entries[cacheKey];
                if (cacheEntry != null)
                {
                    notInCache = cacheEntry.State;
                    if (base._cacheCommon._enableExpiration && (cacheEntry.UtcExpires < utcNow))
                    {
                        if (flag2)
                        {
                            if (notInCache == CacheEntry.EntryState.AddedToCache)
                            {
                                flag4 = true;
                                goto Label_003E;
                            }
                            cacheEntry = null;
                        }
                        else
                        {
                            replace = true;
                            removedReason = CacheItemRemovedReason.Expired;
                        }
                    }
                    else
                    {
                        flag5 = base._cacheCommon._enableExpiration && (cacheEntry.SlidingExpiration > TimeSpan.Zero);
                    }
                }
                if (!flag2)
                {
                    if (replace && (cacheEntry != null))
                    {
                        if (notInCache != CacheEntry.EntryState.AddingToCache)
                        {
                            key = cacheEntry;
                            key.State = CacheEntry.EntryState.RemovingFromCache;
                            this._entries.Remove(key);
                        }
                        else if (newEntry == null)
                        {
                            cacheEntry = null;
                        }
                    }
                    if (newEntry != null)
                    {
                        bool flag9 = true;
                        if ((cacheEntry != null) && (key == null))
                        {
                            flag9 = false;
                            removed = CacheItemRemovedReason.Removed;
                        }
                        if (flag9)
                        {
                            dependency = newEntry.Dependency;
                            if ((dependency != null) && dependency.HasChanged)
                            {
                                flag9 = false;
                                removed = CacheItemRemovedReason.DependencyChanged;
                            }
                        }
                        if (flag9)
                        {
                            newEntry.State = CacheEntry.EntryState.AddingToCache;
                            this._entries.Add(newEntry, newEntry);
                            if (flag3)
                            {
                                cacheEntry = null;
                            }
                            else
                            {
                                cacheEntry = newEntry;
                            }
                        }
                        else
                        {
                            if (!flag3)
                            {
                                cacheEntry = null;
                                flag6 = true;
                            }
                            else
                            {
                                flag6 = cacheEntry == null;
                            }
                            if (!flag6)
                            {
                                newEntry = null;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (flag7)
                {
                    Monitor.Exit(this._lock);
                }
            }
            if (flag2)
            {
                if (cacheEntry != null)
                {
                    if (flag5)
                    {
                        minValue = utcNow + cacheEntry.SlidingExpiration;
                        if (((minValue - cacheEntry.UtcExpires) >= CacheExpires.MIN_UPDATE_DELTA) || (minValue < cacheEntry.UtcExpires))
                        {
                            this._expires.UtcUpdate(cacheEntry, minValue);
                        }
                    }
                    this.UtcUpdateUsageRecursive(cacheEntry, utcNow);
                }
                if (cacheKey.IsPublic)
                {
                    PerfCounters.IncrementCounter(AppPerfCounter.API_CACHE_RATIO_BASE);
                    if (cacheEntry != null)
                    {
                        PerfCounters.IncrementCounter(AppPerfCounter.API_CACHE_HITS);
                    }
                    else
                    {
                        PerfCounters.IncrementCounter(AppPerfCounter.API_CACHE_MISSES);
                    }
                }
                PerfCounters.IncrementCounter(AppPerfCounter.TOTAL_CACHE_RATIO_BASE);
                if (cacheEntry != null)
                {
                    PerfCounters.IncrementCounter(AppPerfCounter.TOTAL_CACHE_HITS);
                    return cacheEntry;
                }
                PerfCounters.IncrementCounter(AppPerfCounter.TOTAL_CACHE_MISSES);
                return cacheEntry;
            }
            int num = 0;
            int num2 = 0;
            int delta = 0;
            int num4 = 0;
            if (key != null)
            {
                if (key.InExpires())
                {
                    this._expires.Remove(key);
                }
                if (key.InUsage())
                {
                    this._usage.Remove(key);
                }
                key.State = CacheEntry.EntryState.RemovedFromCache;
                valueOld = key.Value;
                num--;
                delta++;
                if (key.IsPublic)
                {
                    num2--;
                    num4++;
                }
            }
            if (newEntry != null)
            {
                if (flag6)
                {
                    newEntry.State = CacheEntry.EntryState.RemovedFromCache;
                    newEntry.Close(removed);
                    newEntry = null;
                }
                else
                {
                    if (base._cacheCommon._enableExpiration && newEntry.HasExpiration())
                    {
                        this._expires.Add(newEntry);
                    }
                    if ((base._cacheCommon._enableMemoryCollection && newEntry.HasUsage()) && ((!newEntry.HasExpiration() || (newEntry.SlidingExpiration > TimeSpan.Zero)) || ((newEntry.UtcExpires - utcNow) >= CacheUsage.MIN_LIFETIME_FOR_USAGE)))
                    {
                        this._usage.Add(newEntry);
                    }
                    newEntry.State = CacheEntry.EntryState.AddedToCache;
                    num++;
                    delta++;
                    if (newEntry.IsPublic)
                    {
                        num2++;
                        num4++;
                    }
                }
            }
            if (key != null)
            {
                key.Close(removedReason);
            }
            if (newEntry != null)
            {
                newEntry.MonitorDependencyChanges();
                if ((dependency != null) && dependency.HasChanged)
                {
                    base.Remove(newEntry, CacheItemRemovedReason.DependencyChanged);
                }
            }
            switch (num)
            {
                case 1:
                    Interlocked.Increment(ref this._totalCount);
                    PerfCounters.IncrementCounter(AppPerfCounter.TOTAL_CACHE_ENTRIES);
                    break;

                case -1:
                    Interlocked.Decrement(ref this._totalCount);
                    PerfCounters.DecrementCounter(AppPerfCounter.TOTAL_CACHE_ENTRIES);
                    break;
            }
            switch (num2)
            {
                case 1:
                    Interlocked.Increment(ref this._publicCount);
                    PerfCounters.IncrementCounter(AppPerfCounter.API_CACHE_ENTRIES);
                    break;

                case -1:
                    Interlocked.Decrement(ref this._publicCount);
                    PerfCounters.DecrementCounter(AppPerfCounter.API_CACHE_ENTRIES);
                    break;
            }
            if (delta > 0)
            {
                PerfCounters.IncrementCounterEx(AppPerfCounter.TOTAL_CACHE_TURNOVER_RATE, delta);
            }
            if (num4 > 0)
            {
                PerfCounters.IncrementCounterEx(AppPerfCounter.API_CACHE_TURNOVER_RATE, num4);
            }
            return cacheEntry;
        }

        private ManualResetEvent UseInsertBlock()
        {
            int num;
            do
            {
                if (this._disposed == 1)
                {
                    return null;
                }
                num = this._insertBlockCalls;
                if (num < 0)
                {
                    return null;
                }
            }
            while (Interlocked.CompareExchange(ref this._insertBlockCalls, num + 1, num) != num);
            return this._insertBlock;
        }

        private void UtcUpdateUsageRecursive(CacheEntry entry, DateTime utcNow)
        {
            if (((utcNow - entry.UtcLastUsageUpdate) > CacheUsage.CORRELATED_REQUEST_TIMEOUT) || (utcNow < entry.UtcLastUsageUpdate))
            {
                entry.UtcLastUsageUpdate = utcNow;
                if (entry.InUsage())
                {
                    CacheSingle cacheSingle;
                    if (this._cacheMultiple == null)
                    {
                        cacheSingle = this;
                    }
                    else
                    {
                        cacheSingle = this._cacheMultiple.GetCacheSingle(entry.Key.GetHashCode());
                    }
                    cacheSingle._usage.Update(entry);
                }
                CacheDependency dependency = entry.Dependency;
                if (dependency != null)
                {
                    CacheEntry[] cacheEntries = dependency.CacheEntries;
                    if (cacheEntries != null)
                    {
                        foreach (CacheEntry entry2 in cacheEntries)
                        {
                            this.UtcUpdateUsageRecursive(entry2, utcNow);
                        }
                    }
                }
            }
        }

        private bool WaitInsertBlock()
        {
            bool flag = false;
            ManualResetEvent event2 = null;
            try
            {
                event2 = this.UseInsertBlock();
                if (event2 != null)
                {
                    flag = event2.WaitOne(INSERT_BLOCK_WAIT, false);
                }
            }
            finally
            {
                if (event2 != null)
                {
                    this.ReleaseInsertBlock();
                }
            }
            return flag;
        }

        internal override int PublicCount =>
            this._publicCount;
    }
}

