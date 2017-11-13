namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.Internal;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class ObjectQuery : IOrderedQueryable, IQueryable, IEnumerable, IListSource
    {
        private TypeUsage _resultType;
        private ObjectQueryState _state;

        internal ObjectQuery(ObjectQueryState queryState)
        {
            this._state = queryState;
            this._state.PlanCachingEnabled = true;
        }

        public ObjectResult Execute(System.Data.Objects.MergeOption mergeOption)
        {
            EntityUtil.CheckArgumentMergeOption(mergeOption);
            return this.ExecuteInternal(mergeOption);
        }

        internal abstract ObjectResult ExecuteInternal(System.Data.Objects.MergeOption mergeOption);
        internal abstract IEnumerator GetEnumeratorInternal();
        internal abstract IList GetIListSourceListInternal();
        public TypeUsage GetResultType()
        {
            this.Context.EnsureMetadata();
            if (this._resultType == null)
            {
                TypeUsage oSpaceTypeUsage;
                TypeUsage resultType = this._state.ResultType;
                if (!TypeHelpers.TryGetCollectionElementType(resultType, out oSpaceTypeUsage))
                {
                    oSpaceTypeUsage = resultType;
                }
                oSpaceTypeUsage = this._state.ObjectContext.Perspective.MetadataWorkspace.GetOSpaceTypeUsage(oSpaceTypeUsage);
                if (oSpaceTypeUsage == null)
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ObjectQuery_UnableToMapResultType);
                }
                this._resultType = oSpaceTypeUsage;
            }
            return this._resultType;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumeratorInternal();

        IList IListSource.GetList() => 
            this.GetIListSourceListInternal();

        [Browsable(false)]
        public string ToTraceString() => 
            this._state.GetExecutionPlan(null).ToTraceString();

        internal void Validate()
        {
            this._state.GetExecutionPlan(null);
        }

        public string CommandText
        {
            get
            {
                string str;
                if (!this._state.TryGetCommandText(out str))
                {
                    return string.Empty;
                }
                return str;
            }
        }

        public ObjectContext Context =>
            this._state.ObjectContext;

        public bool EnablePlanCaching
        {
            get => 
                this._state.PlanCachingEnabled;
            set
            {
                this._state.PlanCachingEnabled = value;
            }
        }

        public System.Data.Objects.MergeOption MergeOption
        {
            get => 
                this._state.EffectiveMergeOption;
            set
            {
                EntityUtil.CheckArgumentMergeOption(value);
                this._state.UserSpecifiedMergeOption = new System.Data.Objects.MergeOption?(value);
            }
        }

        public ObjectParameterCollection Parameters =>
            this._state.EnsureParameters();

        internal ObjectQueryState QueryState =>
            this._state;

        bool IListSource.ContainsListCollection =>
            false;

        Type IQueryable.ElementType =>
            this._state.ElementType;

        Expression IQueryable.Expression
        {
            get
            {
                Expression expression;
                if (!this._state.TryGetExpression(out expression))
                {
                    expression = Expression.Constant(this);
                }
                return expression;
            }
        }

        IQueryProvider IQueryable.Provider =>
            this.Context.Provider;
    }
}

