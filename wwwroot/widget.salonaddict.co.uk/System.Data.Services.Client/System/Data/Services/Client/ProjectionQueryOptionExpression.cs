namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class ProjectionQueryOptionExpression : QueryOptionExpression
    {
        private readonly LambdaExpression lambda;
        private readonly List<string> paths;

        internal ProjectionQueryOptionExpression(Type type, LambdaExpression lambda, List<string> paths) : base((ExpressionType) 0x2718, type)
        {
            this.lambda = lambda;
            this.paths = paths;
        }

        internal List<string> Paths =>
            this.paths;

        internal LambdaExpression Selector =>
            this.lambda;
    }
}

