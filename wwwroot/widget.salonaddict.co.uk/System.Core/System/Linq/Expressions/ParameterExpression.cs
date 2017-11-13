namespace System.Linq.Expressions
{
    using System;
    using System.Text;

    public sealed class ParameterExpression : Expression
    {
        private string name;

        internal ParameterExpression(Type type, string name) : base(ExpressionType.Parameter, type)
        {
            this.name = name;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            if (this.name != null)
            {
                builder.Append(this.name);
            }
            else
            {
                builder.Append("<param>");
            }
        }

        public string Name =>
            this.name;
    }
}

