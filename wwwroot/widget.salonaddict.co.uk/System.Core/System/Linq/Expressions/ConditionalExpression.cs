namespace System.Linq.Expressions
{
    using System;
    using System.Text;

    public sealed class ConditionalExpression : Expression
    {
        private Expression ifFalse;
        private Expression ifTrue;
        private Expression test;

        internal ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse, Type type) : base(ExpressionType.Conditional, type)
        {
            this.test = test;
            this.ifTrue = ifTrue;
            this.ifFalse = ifFalse;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            builder.Append("IIF(");
            this.test.BuildString(builder);
            builder.Append(", ");
            this.ifTrue.BuildString(builder);
            builder.Append(", ");
            this.ifFalse.BuildString(builder);
            builder.Append(")");
        }

        public Expression IfFalse =>
            this.ifFalse;

        public Expression IfTrue =>
            this.ifTrue;

        public Expression Test =>
            this.test;
    }
}

