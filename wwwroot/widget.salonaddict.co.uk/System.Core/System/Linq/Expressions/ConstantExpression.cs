namespace System.Linq.Expressions
{
    using System;
    using System.Text;

    public sealed class ConstantExpression : Expression
    {
        private object value;

        internal ConstantExpression(object value, Type type) : base(ExpressionType.Constant, type)
        {
            this.value = value;
        }

        internal override void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw Error.ArgumentNull("builder");
            }
            if (this.value != null)
            {
                if (this.value is string)
                {
                    builder.Append("\"");
                    builder.Append(this.value);
                    builder.Append("\"");
                }
                else if (this.value.ToString() == this.value.GetType().ToString())
                {
                    builder.Append("value(");
                    builder.Append(this.value);
                    builder.Append(")");
                }
                else
                {
                    builder.Append(this.value);
                }
            }
            else
            {
                builder.Append("null");
            }
        }

        public object Value =>
            this.value;
    }
}

