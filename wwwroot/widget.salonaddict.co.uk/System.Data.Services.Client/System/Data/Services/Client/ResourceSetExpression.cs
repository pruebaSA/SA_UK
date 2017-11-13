namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    [DebuggerDisplay("ResourceSetExpression {Source}.{MemberExpression}")]
    internal class ResourceSetExpression : ResourceExpression
    {
        private Dictionary<PropertyInfo, ConstantExpression> keyFilter;
        private readonly Expression member;
        private readonly Type resourceType;
        private List<QueryOptionExpression> sequenceQueryOptions;
        private TransparentAccessors transparentScope;

        internal ResourceSetExpression(Type type, Expression source, Expression memberExpression, Type resourceType, List<string> expandPaths, CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection) : base(source, (source != null) ? ((ExpressionType) 0x2711) : ((ExpressionType) 0x2710), type, expandPaths, countOption, customQueryOptions, projection)
        {
            this.member = memberExpression;
            this.resourceType = resourceType;
            this.sequenceQueryOptions = new List<QueryOptionExpression>();
        }

        internal void AddSequenceQueryOption(QueryOptionExpression qoe)
        {
            QueryOptionExpression previous = (from o in this.sequenceQueryOptions
                where o.GetType() == qoe.GetType()
                select o).FirstOrDefault<QueryOptionExpression>();
            if (previous != null)
            {
                qoe = qoe.ComposeMultipleSpecification(previous);
                this.sequenceQueryOptions.Remove(previous);
            }
            this.sequenceQueryOptions.Add(qoe);
        }

        internal override ResourceExpression CreateCloneWithNewType(Type type) => 
            new ResourceSetExpression(type, base.source, this.MemberExpression, TypeSystem.GetElementType(type), this.ExpandPaths.ToList<string>(), this.CountOption, this.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>(kvp => kvp.Key, kvp => kvp.Value), base.Projection) { 
                keyFilter = this.keyFilter,
                sequenceQueryOptions = this.sequenceQueryOptions,
                transparentScope = this.transparentScope
            };

        internal void OverrideInputReference(ResourceSetExpression newInput)
        {
            InputReferenceExpression inputRef = newInput.inputRef;
            if (inputRef != null)
            {
                base.inputRef = inputRef;
                inputRef.OverrideTarget(this);
            }
        }

        internal FilterQueryOptionExpression Filter =>
            this.sequenceQueryOptions.OfType<FilterQueryOptionExpression>().SingleOrDefault<FilterQueryOptionExpression>();

        internal bool HasKeyPredicate =>
            (this.keyFilter != null);

        internal override bool HasQueryOptions
        {
            get
            {
                if (((this.sequenceQueryOptions.Count <= 0) && (this.ExpandPaths.Count <= 0)) && ((this.CountOption != CountOption.InlineAll) && (this.CustomQueryOptions.Count <= 0)))
                {
                    return (base.Projection != null);
                }
                return true;
            }
        }

        internal bool HasSequenceQueryOptions =>
            (this.sequenceQueryOptions.Count > 0);

        internal bool HasTransparentScope =>
            (this.transparentScope != null);

        internal override bool IsSingleton =>
            this.HasKeyPredicate;

        internal Dictionary<PropertyInfo, ConstantExpression> KeyPredicate
        {
            get => 
                this.keyFilter;
            set
            {
                this.keyFilter = value;
            }
        }

        internal Expression MemberExpression =>
            this.member;

        internal OrderByQueryOptionExpression OrderBy =>
            this.sequenceQueryOptions.OfType<OrderByQueryOptionExpression>().SingleOrDefault<OrderByQueryOptionExpression>();

        internal override Type ResourceType =>
            this.resourceType;

        internal IEnumerable<QueryOptionExpression> SequenceQueryOptions =>
            this.sequenceQueryOptions.ToList<QueryOptionExpression>();

        internal SkipQueryOptionExpression Skip =>
            this.sequenceQueryOptions.OfType<SkipQueryOptionExpression>().SingleOrDefault<SkipQueryOptionExpression>();

        internal TakeQueryOptionExpression Take =>
            this.sequenceQueryOptions.OfType<TakeQueryOptionExpression>().SingleOrDefault<TakeQueryOptionExpression>();

        internal TransparentAccessors TransparentScope
        {
            get => 
                this.transparentScope;
            set
            {
                this.transparentScope = value;
            }
        }

        [DebuggerDisplay("{ToString()}")]
        internal class TransparentAccessors
        {
            internal readonly string Accessor;
            internal readonly Dictionary<string, Expression> SourceAccessors;

            internal TransparentAccessors(string acc, Dictionary<string, Expression> sourceAccesors)
            {
                this.Accessor = acc;
                this.SourceAccessors = sourceAccesors;
            }

            public override string ToString() => 
                (("SourceAccessors=[" + string.Join(",", this.SourceAccessors.Keys.ToArray<string>())) + "] ->* Accessor=" + this.Accessor);
        }
    }
}

