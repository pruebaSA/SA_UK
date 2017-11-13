namespace System.Linq.Expressions
{
    using System;
    using System.Text;

    public sealed class TypeBinaryExpression : System.Linq.Expressions.Expression
    {
        private System.Linq.Expressions.Expression expression;
        private Type typeop;

        internal TypeBinaryExpression(ExpressionType nt, System.Linq.Expressions.Expression expression, Type typeop, Type resultType) : base(nt, resultType)
        {
            this.expression = expression;
            this.typeop = typeop;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            builder.Append("(");
            this.expression.BuildString(builder);
            builder.Append(" Is ");
            builder.Append(this.typeop.Name);
            builder.Append(")");
        }

        public System.Linq.Expressions.Expression Expression =>
            this.expression;

        public Type TypeOperand =>
            this.typeop;
    }
}

