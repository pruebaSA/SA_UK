namespace System.Data.Services.Client
{
    using System;
    using System.Linq.Expressions;

    internal class FilterQueryOptionExpression : QueryOptionExpression
    {
        private Expression predicate;

        internal FilterQueryOptionExpression(Type type, Expression predicate) : base((ExpressionType) 0x2716, type)
        {
            this.predicate = predicate;
        }

        internal Expression Predicate =>
            this.predicate;
    }
}

