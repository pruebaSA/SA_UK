namespace System.Data.Common.QueryCache
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.Internal;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal sealed class CompiledQueryCacheEntry : QueryCacheEntry
    {
        private ObjectQueryExecutionPlan _appendOnlyPlan;
        private ObjectQueryExecutionPlan _noTrackingPlan;
        private ObjectQueryExecutionPlan _overwriteChangesPlan;
        private ObjectQueryExecutionPlan _preserveChangesPlan;
        public readonly MergeOption? PropagatedMergeOption;

        internal CompiledQueryCacheEntry(QueryCacheKey queryCacheKey, MergeOption? mergeOption) : base(queryCacheKey, null)
        {
            this.PropagatedMergeOption = mergeOption;
        }

        internal ObjectQueryExecutionPlan GetExecutionPlan(MergeOption mergeOption)
        {
            switch (mergeOption)
            {
                case MergeOption.AppendOnly:
                    return this._appendOnlyPlan;

                case MergeOption.OverwriteChanges:
                    return this._overwriteChangesPlan;

                case MergeOption.PreserveChanges:
                    return this._preserveChangesPlan;

                case MergeOption.NoTracking:
                    return this._noTrackingPlan;
            }
            throw EntityUtil.ArgumentOutOfRange("mergeOption");
        }

        internal ObjectQueryExecutionPlan SetExecutionPlan(ObjectQueryExecutionPlan newPlan)
        {
            ObjectQueryExecutionPlan plan;
            switch (newPlan.MergeOption)
            {
                case MergeOption.AppendOnly:
                    plan = Interlocked.CompareExchange<ObjectQueryExecutionPlan>(ref this._appendOnlyPlan, newPlan, null);
                    break;

                case MergeOption.OverwriteChanges:
                    plan = Interlocked.CompareExchange<ObjectQueryExecutionPlan>(ref this._overwriteChangesPlan, newPlan, null);
                    break;

                case MergeOption.PreserveChanges:
                    plan = Interlocked.CompareExchange<ObjectQueryExecutionPlan>(ref this._preserveChangesPlan, newPlan, null);
                    break;

                case MergeOption.NoTracking:
                    plan = Interlocked.CompareExchange<ObjectQueryExecutionPlan>(ref this._noTrackingPlan, newPlan, null);
                    break;

                default:
                    throw EntityUtil.ArgumentOutOfRange("newPlan.MergeOption");
            }
            return (plan ?? newPlan);
        }

        internal bool TryGetResultType(out TypeUsage resultType)
        {
            ObjectQueryExecutionPlan plan = this._appendOnlyPlan;
            if (plan != null)
            {
                resultType = plan.ResultType;
                return true;
            }
            plan = this._noTrackingPlan;
            if (plan != null)
            {
                resultType = plan.ResultType;
                return true;
            }
            plan = this._overwriteChangesPlan;
            if (plan != null)
            {
                resultType = plan.ResultType;
                return true;
            }
            plan = this._preserveChangesPlan;
            if (plan != null)
            {
                resultType = plan.ResultType;
                return true;
            }
            resultType = null;
            return false;
        }
    }
}

