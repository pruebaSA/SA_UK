namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class QueryComponents
    {
        private readonly Type lastSegmentType;
        private readonly Dictionary<Expression, Expression> normalizerRewrites;
        private readonly LambdaExpression projection;
        private readonly System.Uri uri;
        private System.Version version;

        internal QueryComponents(System.Uri uri, System.Version version, Type lastSegmentType, LambdaExpression projection, Dictionary<Expression, Expression> normalizerRewrites)
        {
            this.projection = projection;
            this.normalizerRewrites = normalizerRewrites;
            this.lastSegmentType = lastSegmentType;
            this.uri = uri;
            this.version = version;
        }

        internal Type LastSegmentType =>
            this.lastSegmentType;

        internal Dictionary<Expression, Expression> NormalizerRewrites =>
            this.normalizerRewrites;

        internal LambdaExpression Projection =>
            this.projection;

        internal System.Uri Uri =>
            this.uri;

        internal System.Version Version
        {
            get => 
                this.version;
            set
            {
                this.version = value;
            }
        }
    }
}

