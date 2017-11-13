namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class UnaryExpression : Expression
    {
        private MethodInfo method;
        private Expression operand;

        internal UnaryExpression(ExpressionType nt, Expression operand, Type type) : this(nt, operand, null, type)
        {
        }

        internal UnaryExpression(ExpressionType nt, Expression operand, MethodInfo method, Type type) : base(nt, type)
        {
            this.operand = operand;
            this.method = method;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            switch (base.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    builder.Append("-");
                    this.operand.BuildString(builder);
                    return;

                case ExpressionType.UnaryPlus:
                    builder.Append("+");
                    this.operand.BuildString(builder);
                    return;

                case ExpressionType.Not:
                    builder.Append("Not");
                    builder.Append("(");
                    this.operand.BuildString(builder);
                    builder.Append(")");
                    return;

                case ExpressionType.Quote:
                    this.operand.BuildString(builder);
                    return;

                case ExpressionType.TypeAs:
                    builder.Append("(");
                    this.operand.BuildString(builder);
                    builder.Append(" As ");
                    builder.Append(base.Type.Name);
                    builder.Append(")");
                    return;
            }
            builder.Append(base.NodeType);
            builder.Append("(");
            this.operand.BuildString(builder);
            builder.Append(")");
        }

        public bool IsLifted
        {
            get
            {
                if ((base.NodeType == ExpressionType.TypeAs) || (base.NodeType == ExpressionType.Quote))
                {
                    return false;
                }
                bool flag = Expression.IsNullableType(this.operand.Type);
                bool flag2 = Expression.IsNullableType(base.Type);
                if (this.method != null)
                {
                    return ((flag && (this.method.GetParameters()[0].ParameterType != this.operand.Type)) || (flag2 && (this.method.ReturnType != base.Type)));
                }
                if (!flag)
                {
                    return flag2;
                }
                return true;
            }
        }

        public bool IsLiftedToNull =>
            (this.IsLifted && Expression.IsNullableType(base.Type));

        public MethodInfo Method =>
            this.method;

        public Expression Operand =>
            this.operand;
    }
}

