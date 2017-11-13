namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data.Objects.Internal;

    internal sealed class EntitySqlQueryCacheEntry : QueryCacheEntry
    {
        private readonly ObjectQueryExecutionPlan _plan;

        internal EntitySqlQueryCacheEntry(QueryCacheKey queryCacheKey, ObjectQueryExecutionPlan plan) : base(queryCacheKey, plan)
        {
            this._plan = plan;
        }

        internal ObjectQueryExecutionPlan ExecutionPlan =>
            this._plan;
    }
}

