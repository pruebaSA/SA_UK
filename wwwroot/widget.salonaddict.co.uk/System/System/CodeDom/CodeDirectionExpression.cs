namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeDirectionExpression : CodeExpression
    {
        private FieldDirection direction;
        private CodeExpression expression;

        public CodeDirectionExpression()
        {
        }

        public CodeDirectionExpression(FieldDirection direction, CodeExpression expression)
        {
            this.expression = expression;
            this.direction = direction;
        }

        public FieldDirection Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = value;
            }
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

