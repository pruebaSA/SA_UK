namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text;

    public sealed class ElementInit
    {
        private MethodInfo addMethod;
        private ReadOnlyCollection<Expression> arguments;

        internal ElementInit(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            this.addMethod = addMethod;
            this.arguments = arguments;
        }

        internal void BuildString(StringBuilder builder)
        {
            builder.Append(this.AddMethod);
            builder.Append("(");
            bool flag = true;
            foreach (Expression expression in this.arguments)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(",");
                }
                expression.BuildString(builder);
            }
            builder.Append(")");
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.BuildString(builder);
            return builder.ToString();
        }

        public MethodInfo AddMethod =>
            this.addMethod;

        public ReadOnlyCollection<Expression> Arguments =>
            this.arguments;
    }
}

