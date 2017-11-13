namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    public sealed class ListInitExpression : Expression
    {
        private ReadOnlyCollection<ElementInit> initializers;
        private System.Linq.Expressions.NewExpression newExpression;

        internal ListInitExpression(System.Linq.Expressions.NewExpression newExpression, ReadOnlyCollection<ElementInit> initializers) : base(ExpressionType.ListInit, newExpression.Type)
        {
            this.newExpression = newExpression;
            this.initializers = initializers;
        }

        internal override void BuildString(StringBuilder builder)
        {
            this.newExpression.BuildString(builder);
            builder.Append(" {");
            int num = 0;
            int count = this.initializers.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    builder.Append(", ");
                }
                this.initializers[num].BuildString(builder);
                num++;
            }
            builder.Append("}");
        }

        public ReadOnlyCollection<ElementInit> Initializers =>
            this.initializers;

        public System.Linq.Expressions.NewExpression NewExpression =>
            this.newExpression;
    }
}

