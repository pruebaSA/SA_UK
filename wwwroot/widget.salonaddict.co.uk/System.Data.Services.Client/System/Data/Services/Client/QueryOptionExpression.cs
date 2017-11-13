namespace System.Data.Services.Client
{
    using System;
    using System.Linq.Expressions;

    internal abstract class QueryOptionExpression : Expression
    {
        internal QueryOptionExpression(ExpressionType nodeType, Type type) : base(nodeType, type)
        {
        }

        internal virtual QueryOptionExpression ComposeMultipleSpecification(QueryOptionExpression previous) => 
            this;
    }
}

