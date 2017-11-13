namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;

    public class LambdaExpression : Expression
    {
        private Expression body;
        private ReadOnlyCollection<ParameterExpression> parameters;

        internal LambdaExpression(Expression body, Type type, ReadOnlyCollection<ParameterExpression> parameters) : base(ExpressionType.Lambda, type)
        {
            this.body = body;
            this.parameters = parameters;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (this.Parameters.Count == 1)
            {
                this.Parameters[0].BuildString(builder);
            }
            else
            {
                builder.Append("(");
                int num = 0;
                int count = this.Parameters.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        builder.Append(", ");
                    }
                    this.Parameters[num].BuildString(builder);
                    num++;
                }
                builder.Append(")");
            }
            builder.Append(" => ");
            this.body.BuildString(builder);
        }

        public Delegate Compile()
        {
            ExpressionCompiler compiler = new ExpressionCompiler();
            return compiler.Compile(this);
        }

        public Expression Body =>
            this.body;

        public ReadOnlyCollection<ParameterExpression> Parameters =>
            this.parameters;
    }
}

