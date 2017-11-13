namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data.Common.Internal.Materialization;

    internal class ShaperFactoryQueryCacheEntry<T> : QueryCacheEntry
    {
        internal ShaperFactoryQueryCacheEntry(ShaperFactoryQueryCacheKey<T> key, ShaperFactory<T> factory) : base(key, factory)
        {
        }

        internal ShaperFactory<T> GetTarget() => 
            ((ShaperFactory<T>) base.GetTarget());
    }
}

