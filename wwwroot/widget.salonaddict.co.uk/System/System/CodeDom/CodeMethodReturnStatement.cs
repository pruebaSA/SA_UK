namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeMethodReturnStatement : CodeStatement
    {
        private CodeExpression expression;

        public CodeMethodReturnStatement()
        {
        }

        public CodeMethodReturnStatement(CodeExpression expression)
        {
            this.Expression = expression;
        }

        public CodeExpression Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }
    }
}

