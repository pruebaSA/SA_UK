namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Text;

    public sealed class BinaryExpression : Expression
    {
        private LambdaExpression conversion;
        private Expression left;
        private MethodInfo method;
        private Expression right;

        internal BinaryExpression(ExpressionType nt, Expression left, Expression right, Type type) : this(nt, left, right, null, null, type)
        {
        }

        internal BinaryExpression(ExpressionType nt, Expression left, Expression right, LambdaExpression conversion, Type type) : this(nt, left, right, null, conversion, type)
        {
        }

        internal BinaryExpression(ExpressionType nt, Expression left, Expression right, MethodInfo method, Type type) : this(nt, left, right, method, null, type)
        {
        }

        internal BinaryExpression(ExpressionType nt, Expression left, Expression right, MethodInfo method, LambdaExpression conversion, Type type) : base(nt, type)
        {
            this.left = left;
            this.right = right;
            this.method = method;
            this.conversion = conversion;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            if (base.NodeType == ExpressionType.ArrayIndex)
            {
                this.left.BuildString(builder);
                builder.Append("[");
                this.right.BuildString(builder);
                builder.Append("]");
            }
            else
            {
                string @operator = this.GetOperator();
                if (@operator != null)
                {
                    builder.Append("(");
                    this.left.BuildString(builder);
                    builder.Append(" ");
                    builder.Append(@operator);
                    builder.Append(" ");
                    this.right.BuildString(builder);
                    builder.Append(")");
                }
                else
                {
                    builder.Append(base.NodeType);
                    builder.Append("(");
                    this.left.BuildString(builder);
                    builder.Append(", ");
                    this.right.BuildString(builder);
                    builder.Append(")");
                }
            }
        }

        private string GetOperator()
        {
            switch (base.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";

                case ExpressionType.And:
                    if ((base.Type != typeof(bool)) && (base.Type != typeof(bool?)))
                    {
                        return "&";
                    }
                    return "And";

                case ExpressionType.AndAlso:
                    return "&&";

                case ExpressionType.Coalesce:
                    return "??";

                case ExpressionType.Divide:
                    return "/";

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.ExclusiveOr:
                    return "^";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.LeftShift:
                    return "<<";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.Modulo:
                    return "%";

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";

                case ExpressionType.NotEqual:
                    return "!=";

                case ExpressionType.Or:
                    if ((base.Type != typeof(bool)) && (base.Type != typeof(bool?)))
                    {
                        return "|";
                    }
                    return "Or";

                case ExpressionType.OrElse:
                    return "||";

                case ExpressionType.Power:
                    return "^";

                case ExpressionType.RightShift:
                    return ">>";

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
            }
            return null;
        }

        public LambdaExpression Conversion =>
            this.conversion;

        public bool IsLifted
        {
            get
            {
                if (base.NodeType == ExpressionType.Coalesce)
                {
                    return false;
                }
                bool flag = Expression.IsNullableType(this.left.Type);
                if (this.method == null)
                {
                    return flag;
                }
                return (flag && (this.method.GetParameters()[0].ParameterType != this.left.Type));
            }
        }

        public bool IsLiftedToNull =>
            (this.IsLifted && Expression.IsNullableType(base.Type));

        public Expression Left =>
            this.left;

        public MethodInfo Method =>
            this.method;

        public Expression Right =>
            this.right;
    }
}

