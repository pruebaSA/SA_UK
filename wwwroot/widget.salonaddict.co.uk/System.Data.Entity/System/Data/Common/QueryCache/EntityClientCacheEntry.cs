namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data.EntityClient;

    internal sealed class EntityClientCacheEntry : QueryCacheEntry
    {
        internal EntityClientCacheEntry(QueryCacheKey queryCacheKey, EntityCommandDefinition target) : base(queryCacheKey, target)
        {
        }

        internal EntityCommandDefinition GetTarget() => 
            ((EntityCommandDefinition) base._target);
    }
}

