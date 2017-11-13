namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    public sealed class MemberInitExpression : Expression
    {
        private ReadOnlyCollection<MemberBinding> bindings;
        private System.Linq.Expressions.NewExpression newExpression;

        internal MemberInitExpression(System.Linq.Expressions.NewExpression newExpression, ReadOnlyCollection<MemberBinding> bindings) : base(ExpressionType.MemberInit, newExpression.Type)
        {
            this.newExpression = newExpression;
            this.bindings = bindings;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if ((this.newExpression.Arguments.Count == 0) && this.newExpression.Type.Name.Contains("<"))
            {
                builder.Append("new");
            }
            else
            {
                this.newExpression.BuildString(builder);
            }
            builder.Append(" {");
            int num = 0;
            int count = this.bindings.Count;
            while (num < count)
            {
                MemberBinding binding = this.bindings[num];
                if (num > 0)
                {
                    builder.Append(", ");
                }
                binding.BuildString(builder);
                num++;
            }
            builder.Append("}");
        }

        public ReadOnlyCollection<MemberBinding> Bindings =>
            this.bindings;

        public System.Linq.Expressions.NewExpression NewExpression =>
            this.newExpression;
    }
}

