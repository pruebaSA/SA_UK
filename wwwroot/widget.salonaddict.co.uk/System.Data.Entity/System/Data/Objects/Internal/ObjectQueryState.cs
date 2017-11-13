namespace System.Data.Objects.Internal
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal abstract class ObjectQueryState
    {
        protected ObjectQueryExecutionPlan _cachedPlan;
        private bool _cachingEnabled;
        private readonly System.Data.Objects.ObjectContext _context;
        private readonly Type _elementType;
        private ObjectParameterCollection _parameters;
        private System.Data.Objects.Span _span;
        private MergeOption? _userMergeOption;
        internal static readonly MergeOption DefaultMergeOption;

        protected ObjectQueryState(Type elementType, System.Data.Objects.ObjectContext context, ObjectParameterCollection parameters, System.Data.Objects.Span span)
        {
            EntityUtil.CheckArgumentNull<Type>(elementType, "elementType");
            EntityUtil.CheckArgumentNull<System.Data.Objects.ObjectContext>(context, "context");
            this._elementType = elementType;
            this._context = context;
            this._span = span;
            this._parameters = parameters;
        }

        internal void ApplySettingsTo(ObjectQueryState other)
        {
            other.PlanCachingEnabled = this.PlanCachingEnabled;
            other.UserSpecifiedMergeOption = this.UserSpecifiedMergeOption;
        }

        public static ObjectQuery<TResultType> CreateObjectQuery<TResultType>(ObjectQueryState queryState) => 
            new ObjectQuery<TResultType>(queryState);

        internal ObjectQuery CreateQuery() => 
            ((ObjectQuery) typeof(ObjectQueryState).GetMethod("CreateObjectQuery", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { this._elementType }).Invoke(null, new object[] { this }));

        protected static MergeOption EnsureMergeOption(params MergeOption?[] preferredMergeOptions)
        {
            foreach (MergeOption? nullable in preferredMergeOptions)
            {
                if (nullable.HasValue)
                {
                    return nullable.Value;
                }
            }
            return DefaultMergeOption;
        }

        internal ObjectParameterCollection EnsureParameters()
        {
            if (this._parameters == null)
            {
                this._parameters = new ObjectParameterCollection(this.ObjectContext.Perspective);
                if (this._cachedPlan != null)
                {
                    this._parameters.SetReadOnly(true);
                }
            }
            return this._parameters;
        }

        internal abstract ObjectQueryExecutionPlan GetExecutionPlan(MergeOption? forMergeOption);
        protected static MergeOption? GetMergeOption(params MergeOption?[] preferredMergeOptions)
        {
            foreach (MergeOption? nullable in preferredMergeOptions)
            {
                if (nullable.HasValue)
                {
                    return new MergeOption?(nullable.Value);
                }
            }
            return null;
        }

        protected abstract TypeUsage GetResultType();
        internal abstract ObjectQueryState Include<TElementType>(ObjectQuery<TElementType> sourceQuery, string includePath);
        internal abstract bool TryGetCommandText(out string commandText);
        internal abstract bool TryGetExpression(out Expression expression);

        internal MergeOption EffectiveMergeOption
        {
            get
            {
                if (this._userMergeOption.HasValue)
                {
                    return this._userMergeOption.Value;
                }
                ObjectQueryExecutionPlan plan = this._cachedPlan;
                if (plan != null)
                {
                    return plan.MergeOption;
                }
                return DefaultMergeOption;
            }
        }

        internal Type ElementType =>
            this._elementType;

        internal System.Data.Objects.ObjectContext ObjectContext =>
            this._context;

        internal ObjectParameterCollection Parameters =>
            this._parameters;

        internal bool PlanCachingEnabled
        {
            get => 
                this._cachingEnabled;
            set
            {
                this._cachingEnabled = value;
            }
        }

        internal TypeUsage ResultType
        {
            get
            {
                ObjectQueryExecutionPlan plan = this._cachedPlan;
                if (plan != null)
                {
                    return plan.ResultType;
                }
                return this.GetResultType();
            }
        }

        internal System.Data.Objects.Span Span =>
            this._span;

        internal MergeOption? UserSpecifiedMergeOption
        {
            get => 
                this._userMergeOption;
            set
            {
                this._userMergeOption = value;
            }
        }
    }
}

