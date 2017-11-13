namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    [DebuggerDisplay("SkipQueryOptionExpression {SkipAmount}")]
    internal class SkipQueryOptionExpression : QueryOptionExpression
    {
        private ConstantExpression skipAmount;

        internal SkipQueryOptionExpression(Type type, ConstantExpression skipAmount) : base((ExpressionType) 0x2714, type)
        {
            this.skipAmount = skipAmount;
        }

        internal override QueryOptionExpression ComposeMultipleSpecification(QueryOptionExpression previous)
        {
            int num = (int) this.skipAmount.Value;
            int num2 = (int) ((SkipQueryOptionExpression) previous).skipAmount.Value;
            return new SkipQueryOptionExpression(base.Type, Expression.Constant(num + num2, typeof(int)));
        }

        internal ConstantExpression SkipAmount =>
            this.skipAmount;
    }
}

