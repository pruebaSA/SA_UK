namespace System.ComponentModel.Design.Serialization
{
    using System;
    using System.CodeDom;

    public sealed class RootContext
    {
        private CodeExpression expression;
        private object value;

        public RootContext(CodeExpression expression, object value)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.expression = expression;
            this.value = value;
        }

        public CodeExpression Expression =>
            this.expression;

        public object Value =>
            this.value;
    }
}

