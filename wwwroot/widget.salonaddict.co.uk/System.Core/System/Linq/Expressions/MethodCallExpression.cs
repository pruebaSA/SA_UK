namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public sealed class MethodCallExpression : Expression
    {
        private ReadOnlyCollection<Expression> arguments;
        private MethodInfo method;
        private Expression obj;

        internal MethodCallExpression(ExpressionType type, MethodInfo method, Expression obj, ReadOnlyCollection<Expression> arguments) : base(type, method.ReturnType)
        {
            this.obj = obj;
            this.method = method;
            this.arguments = arguments;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            int num = 0;
            Expression expression = this.obj;
            if (Attribute.GetCustomAttribute(this.method, typeof(ExtensionAttribute)) != null)
            {
                num = 1;
                expression = this.arguments[0];
            }
            if (expression != null)
            {
                expression.BuildString(builder);
                builder.Append(".");
            }
            builder.Append(this.method.Name);
            builder.Append("(");
            int num2 = num;
            int count = this.arguments.Count;
            while (num2 < count)
            {
                if (num2 > num)
                {
                    builder.Append(", ");
                }
                this.arguments[num2].BuildString(builder);
                num2++;
            }
            builder.Append(")");
        }

        public ReadOnlyCollection<Expression> Arguments =>
            this.arguments;

        public MethodInfo Method =>
            this.method;

        public Expression Object =>
            this.obj;
    }
}

