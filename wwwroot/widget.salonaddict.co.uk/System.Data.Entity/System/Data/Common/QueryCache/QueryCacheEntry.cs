namespace System.Data.Common.QueryCache
{
    using System;

    internal abstract class QueryCacheEntry
    {
        private readonly System.Data.Common.QueryCache.QueryCacheKey _queryCacheKey;
        protected readonly object _target;

        protected QueryCacheEntry(System.Data.Common.QueryCache.QueryCacheKey queryCacheKey, object target)
        {
            this._queryCacheKey = queryCacheKey;
            this._target = target;
        }

        internal virtual object GetTarget() => 
            this._target;

        internal System.Data.Common.QueryCache.QueryCacheKey QueryCacheKey =>
            this._queryCacheKey;
    }
}

