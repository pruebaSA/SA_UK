namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.EntitySql;
    using System.Data.Common.QueryCache;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.Internal;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    internal sealed class EntitySqlQueryState : ObjectQueryState
    {
        private readonly bool _allowsLimit;
        private readonly string _queryText;

        internal EntitySqlQueryState(Type elementType, string commandText, bool allowsLimit, ObjectContext context, ObjectParameterCollection parameters, Span span) : base(elementType, context, parameters, span)
        {
            EntityUtil.CheckArgumentNull<string>(commandText, "commandText");
            if (string.IsNullOrEmpty(commandText))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_InvalidEmptyQuery, "commandText");
            }
            this._queryText = commandText;
            this._allowsLimit = allowsLimit;
        }

        internal override ObjectQueryExecutionPlan GetExecutionPlan(MergeOption? forMergeOption)
        {
            base.ObjectContext.EnsureMetadata();
            MergeOption?[] preferredMergeOptions = new MergeOption?[] { forMergeOption, base.UserSpecifiedMergeOption };
            MergeOption mergeOption = ObjectQueryState.EnsureMergeOption(preferredMergeOptions);
            ObjectQueryExecutionPlan executionPlan = base._cachedPlan;
            if (executionPlan != null)
            {
                if (executionPlan.MergeOption == mergeOption)
                {
                    return executionPlan;
                }
                executionPlan = null;
            }
            QueryCacheManager queryCacheManager = null;
            EntitySqlQueryCacheKey objectQueryCacheKey = null;
            if (base.PlanCachingEnabled)
            {
                objectQueryCacheKey = new EntitySqlQueryCacheKey(base.ObjectContext.DefaultContainerName, this._queryText, (base.Parameters == null) ? 0 : base.Parameters.Count, base.Parameters?.GetCacheKey(), base.Span?.GetCacheKey(), mergeOption, base.ElementType);
                queryCacheManager = base.ObjectContext.MetadataWorkspace.GetQueryCacheManager();
                ObjectQueryExecutionPlan execPlan = null;
                if (queryCacheManager.TryCacheLookup(objectQueryCacheKey, out execPlan))
                {
                    executionPlan = execPlan;
                }
            }
            if (executionPlan == null)
            {
                DbQueryCommandTree parseTree = new DbQueryCommandTree(base.ObjectContext.MetadataWorkspace, DataSpace.CSpace);
                parseTree.Query = this.Parse(parseTree);
                executionPlan = ObjectQueryExecutionPlan.Prepare(base.ObjectContext, parseTree, base.ElementType, mergeOption, base.Span);
                if (objectQueryCacheKey != null)
                {
                    EntitySqlQueryCacheEntry inQueryCacheEntry = new EntitySqlQueryCacheEntry(objectQueryCacheKey, executionPlan);
                    QueryCacheEntry outQueryCacheEntry = null;
                    if (queryCacheManager.TryLookupAndAdd(inQueryCacheEntry, out outQueryCacheEntry))
                    {
                        executionPlan = ((EntitySqlQueryCacheEntry) outQueryCacheEntry).ExecutionPlan;
                    }
                }
            }
            if (base.Parameters != null)
            {
                base.Parameters.SetReadOnly(true);
            }
            base._cachedPlan = executionPlan;
            return executionPlan;
        }

        protected override TypeUsage GetResultType()
        {
            DbQueryCommandTree parseTree = new DbQueryCommandTree(base.ObjectContext.MetadataWorkspace, DataSpace.CSpace);
            return this.Parse(parseTree).ResultType;
        }

        internal override ObjectQueryState Include<TElementType>(ObjectQuery<TElementType> sourceQuery, string includePath)
        {
            ObjectQueryState other = new EntitySqlQueryState(base.ElementType, this._queryText, this._allowsLimit, base.ObjectContext, ObjectParameterCollection.DeepCopy(base.Parameters), Span.IncludeIn(base.Span, includePath));
            base.ApplySettingsTo(other);
            return other;
        }

        internal DbExpression Parse(DbCommandTree parseTree)
        {
            Dictionary<string, TypeUsage> parameters = null;
            if (base.Parameters != null)
            {
                parameters = new Dictionary<string, TypeUsage>(base.Parameters.Count);
                foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) base.Parameters)
                {
                    TypeUsage typeUsage = parameter.TypeUsage;
                    if (typeUsage == null)
                    {
                        base.ObjectContext.Perspective.TryGetTypeByName(parameter.MappableType.FullName, false, out typeUsage);
                    }
                    parameters.Add(parameter.Name, typeUsage);
                }
            }
            return CqlQuery.Compile(parseTree, this._queryText, base.ObjectContext.Perspective, null, parameters, null);
        }

        internal override bool TryGetCommandText(out string commandText)
        {
            commandText = this._queryText;
            return true;
        }

        internal override bool TryGetExpression(out Expression expression)
        {
            expression = null;
            return false;
        }

        internal bool AllowsLimitSubclause =>
            this._allowsLimit;
    }
}

