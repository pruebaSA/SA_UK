namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal abstract class ResourceExpression : Expression
    {
        private System.Data.Services.Client.CountOption countOption;
        private Dictionary<ConstantExpression, ConstantExpression> customQueryOptions;
        private List<string> expandPaths;
        protected InputReferenceExpression inputRef;
        private ProjectionQueryOptionExpression projection;
        protected readonly Expression source;

        internal ResourceExpression(Expression source, ExpressionType nodeType, Type type, List<string> expandPaths, System.Data.Services.Client.CountOption countOption, Dictionary<ConstantExpression, ConstantExpression> customQueryOptions, ProjectionQueryOptionExpression projection) : base(nodeType, type)
        {
            this.expandPaths = expandPaths ?? new List<string>();
            this.countOption = countOption;
            this.customQueryOptions = customQueryOptions ?? new Dictionary<ConstantExpression, ConstantExpression>(ReferenceEqualityComparer<ConstantExpression>.Instance);
            this.projection = projection;
            this.source = source;
        }

        internal abstract ResourceExpression CreateCloneWithNewType(Type type);
        internal InputReferenceExpression CreateReference()
        {
            if (this.inputRef == null)
            {
                this.inputRef = new InputReferenceExpression(this);
            }
            return this.inputRef;
        }

        internal virtual System.Data.Services.Client.CountOption CountOption
        {
            get => 
                this.countOption;
            set
            {
                this.countOption = value;
            }
        }

        internal virtual Dictionary<ConstantExpression, ConstantExpression> CustomQueryOptions
        {
            get => 
                this.customQueryOptions;
            set
            {
                this.customQueryOptions = value;
            }
        }

        internal virtual List<string> ExpandPaths
        {
            get => 
                this.expandPaths;
            set
            {
                this.expandPaths = value;
            }
        }

        internal abstract bool HasQueryOptions { get; }

        internal abstract bool IsSingleton { get; }

        internal ProjectionQueryOptionExpression Projection
        {
            get => 
                this.projection;
            set
            {
                this.projection = value;
            }
        }

        internal abstract Type ResourceType { get; }

        internal Expression Source =>
            this.source;
    }
}

