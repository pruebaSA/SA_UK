namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class MemberAssignment : MemberBinding
    {
        private System.Linq.Expressions.Expression expression;

        internal MemberAssignment(MemberInfo member, System.Linq.Expressions.Expression expression) : base(MemberBindingType.Assignment, member)
        {
            this.expression = expression;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            builder.Append(base.Member.Name);
            builder.Append(" = ");
            this.expression.BuildString(builder);
        }

        public System.Linq.Expressions.Expression Expression =>
            this.expression;
    }
}

