namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class MemberExpression : System.Linq.Expressions.Expression
    {
        private System.Linq.Expressions.Expression expr;
        private MemberInfo member;

        internal MemberExpression(System.Linq.Expressions.Expression expression, MemberInfo member, Type type) : base(ExpressionType.MemberAccess, type)
        {
            this.expr = expression;
            this.member = member;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            if (this.expr != null)
            {
                this.expr.BuildString(builder);
            }
            else
            {
                builder.Append(this.member.DeclaringType.Name);
            }
            builder.Append(".");
            builder.Append(this.member.Name);
        }

        public System.Linq.Expressions.Expression Expression =>
            this.expr;

        public MemberInfo Member =>
            this.member;
    }
}

