namespace System.Data.Common.QueryCache
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Internal.Materialization;
    using System.Data.EntityClient;
    using System.Data.Objects.Internal;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class QueryCacheManager : IDisposable
    {
        private static readonly int[] _agingFactor = new int[] { 1, 1, 2, 4, 8, 0x10 };
        private readonly Dictionary<QueryCacheKey, QueryCacheEntry> _cacheData = new Dictionary<QueryCacheKey, QueryCacheEntry>(0x20);
        private readonly object _cacheDataLock = new object();
        private readonly EvictionTimer _evictionTimer;
        private readonly int _maxNumberOfEntries;
        private readonly int _sweepingTriggerHighMark;
        private static readonly int AgingMaxIndex = (_agingFactor.Length - 1);
        private const float DefaultHighMarkPercentageFactor = 0.8f;
        private const int DefaultMaxNumberOfEntries = 0x3e8;
        private const int DefaultRecyclerPeriodInMilliseconds = 0xea60;

        private QueryCacheManager(int maximumSize, float loadFactor, int recycleMillis)
        {
            this._maxNumberOfEntries = maximumSize;
            this._sweepingTriggerHighMark = (int) (this._maxNumberOfEntries * loadFactor);
            this._evictionTimer = new EvictionTimer(this, recycleMillis);
        }

        private static void CacheRecyclerHandler(object state)
        {
            ((QueryCacheManager) state).SweepCache();
        }

        internal void Clear()
        {
            lock (this._cacheDataLock)
            {
                this._cacheData.Clear();
            }
        }

        internal static QueryCacheManager Create() => 
            new QueryCacheManager(0x3e8, 0.8f, 0xea60);

        public void Dispose()
        {
            if (this._evictionTimer.Stop())
            {
                this.Clear();
            }
        }

        private void SweepCache()
        {
            if (this._evictionTimer.Suspend())
            {
                lock (this._cacheDataLock)
                {
                    if (this._cacheData.Count > this._sweepingTriggerHighMark)
                    {
                        uint num = 0;
                        List<QueryCacheKey> list = new List<QueryCacheKey>(this._cacheData.Count);
                        list.AddRange(this._cacheData.Keys);
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].HitCount == 0)
                            {
                                this._cacheData.Remove(list[i]);
                                num++;
                            }
                            else
                            {
                                int index = list[i].AgingIndex + 1;
                                if (index > AgingMaxIndex)
                                {
                                    index = AgingMaxIndex;
                                }
                                list[i].AgingIndex = index;
                                list[i].HitCount = list[i].HitCount >> _agingFactor[index];
                            }
                        }
                    }
                }
                this._evictionTimer.Resume();
            }
        }

        internal bool TryCacheLookup(CompiledQueryCacheKey objectQueryCacheKey, out CompiledQueryCacheEntry cacheEntry)
        {
            cacheEntry = null;
            QueryCacheEntry queryCacheEntry = null;
            bool flag = this.TryInternalCacheLookup(objectQueryCacheKey, out queryCacheEntry);
            if (flag)
            {
                cacheEntry = (CompiledQueryCacheEntry) queryCacheEntry;
            }
            return flag;
        }

        internal bool TryCacheLookup(EntityClientCacheKey entityClientCacheKey, out EntityCommandDefinition entityCommandDefinition)
        {
            entityCommandDefinition = null;
            QueryCacheEntry queryCacheEntry = null;
            bool flag = this.TryInternalCacheLookup(entityClientCacheKey, out queryCacheEntry);
            if (flag)
            {
                entityCommandDefinition = ((EntityClientCacheEntry) queryCacheEntry).GetTarget();
            }
            return flag;
        }

        internal bool TryCacheLookup(EntitySqlQueryCacheKey objectQueryCacheKey, out ObjectQueryExecutionPlan execPlan)
        {
            execPlan = null;
            QueryCacheEntry queryCacheEntry = null;
            bool flag = this.TryInternalCacheLookup(objectQueryCacheKey, out queryCacheEntry);
            if (flag)
            {
                execPlan = ((EntitySqlQueryCacheEntry) queryCacheEntry).ExecutionPlan;
            }
            return flag;
        }

        internal bool TryCacheLookup<T>(ShaperFactoryQueryCacheKey<T> key, out ShaperFactory<T> factory)
        {
            QueryCacheEntry entry;
            if (this.TryInternalCacheLookup(key, out entry))
            {
                factory = (ShaperFactory<T>) entry.GetTarget();
                return true;
            }
            factory = null;
            return false;
        }

        private bool TryInternalCacheLookup(QueryCacheKey queryCacheKey, out QueryCacheEntry queryCacheEntry)
        {
            queryCacheEntry = null;
            bool flag = false;
            lock (this._cacheDataLock)
            {
                flag = this._cacheData.TryGetValue(queryCacheKey, out queryCacheEntry);
            }
            if (flag)
            {
                queryCacheEntry.QueryCacheKey.UpdateHit();
            }
            return flag;
        }

        internal bool TryLookupAndAdd(QueryCacheEntry inQueryCacheEntry, out QueryCacheEntry outQueryCacheEntry)
        {
            outQueryCacheEntry = null;
            lock (this._cacheDataLock)
            {
                if (!this._cacheData.TryGetValue(inQueryCacheEntry.QueryCacheKey, out outQueryCacheEntry))
                {
                    this._cacheData.Add(inQueryCacheEntry.QueryCacheKey, inQueryCacheEntry);
                    return false;
                }
                outQueryCacheEntry.QueryCacheKey.UpdateHit();
                return true;
            }
        }

        private sealed class EvictionTimer
        {
            private readonly int _period;
            private readonly object _sync = new object();
            private Timer _timer;

            internal EvictionTimer(QueryCacheManager cacheManager, int recyclePeriod)
            {
                this._period = recyclePeriod;
                this._timer = new Timer(new TimerCallback(QueryCacheManager.CacheRecyclerHandler), cacheManager, recyclePeriod, recyclePeriod);
            }

            internal void Resume()
            {
                lock (this._sync)
                {
                    if (this._timer != null)
                    {
                        this._timer.Change(this._period, this._period);
                    }
                }
            }

            internal bool Stop()
            {
                lock (this._sync)
                {
                    if (this._timer != null)
                    {
                        this._timer.Dispose();
                        this._timer = null;
                        return true;
                    }
                    return false;
                }
            }

            internal bool Suspend()
            {
                lock (this._sync)
                {
                    if (this._timer != null)
                    {
                        this._timer.Change(-1, -1);
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}

