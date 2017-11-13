namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    public sealed class NewArrayExpression : Expression
    {
        private ReadOnlyCollection<Expression> expressions;

        internal NewArrayExpression(ExpressionType eType, Type type, ReadOnlyCollection<Expression> expressions) : base(eType, type)
        {
            this.expressions = expressions;
        }

        internal override void BuildString(StringBuilder builder)
        {
            switch (base.NodeType)
            {
                case ExpressionType.NewArrayInit:
                {
                    builder.Append("new ");
                    builder.Append("[] {");
                    int num3 = 0;
                    int count = this.expressions.Count;
                    while (num3 < count)
                    {
                        if (num3 > 0)
                        {
                            builder.Append(", ");
                        }
                        this.expressions[num3].BuildString(builder);
                        num3++;
                    }
                    builder.Append("}");
                    return;
                }
                case ExpressionType.NewArrayBounds:
                {
                    builder.Append("new ");
                    builder.Append(base.Type.ToString());
                    builder.Append("(");
                    int num = 0;
                    int num2 = this.expressions.Count;
                    while (num < num2)
                    {
                        if (num > 0)
                        {
                            builder.Append(", ");
                        }
                        this.expressions[num].BuildString(builder);
                        num++;
                    }
                    builder.Append(")");
                    return;
                }
            }
        }

        public ReadOnlyCollection<Expression> Expressions =>
            this.expressions;
    }
}

