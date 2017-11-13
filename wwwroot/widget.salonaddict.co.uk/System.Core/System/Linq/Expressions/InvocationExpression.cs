namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    public sealed class InvocationExpression : System.Linq.Expressions.Expression
    {
        private ReadOnlyCollection<System.Linq.Expressions.Expression> arguments;
        private System.Linq.Expressions.Expression lambda;

        internal InvocationExpression(System.Linq.Expressions.Expression lambda, Type returnType, ReadOnlyCollection<System.Linq.Expressions.Expression> arguments) : base(ExpressionType.Invoke, returnType)
        {
            this.lambda = lambda;
            this.arguments = arguments;
        }

        internal override void BuildString(StringBuilder builder)
        {
            builder.Append("Invoke(");
            this.lambda.BuildString(builder);
            int num = 0;
            int count = this.arguments.Count;
            while (num < count)
            {
                builder.Append(",");
                this.arguments[num].BuildString(builder);
                num++;
            }
            builder.Append(")");
        }

        public ReadOnlyCollection<System.Linq.Expressions.Expression> Arguments =>
            this.arguments;

        public System.Linq.Expressions.Expression Expression =>
            this.lambda;
    }
}

