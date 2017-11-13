namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodePrimitiveExpression : CodeExpression
    {
        private object value;

        public CodePrimitiveExpression()
        {
        }

        public CodePrimitiveExpression(object value)
        {
            this.Value = value;
        }

        public object Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

