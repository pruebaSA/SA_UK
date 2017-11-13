namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeArrayIndexerExpression : CodeExpression
    {
        private CodeExpressionCollection indices;
        private CodeExpression targetObject;

        public CodeArrayIndexerExpression()
        {
        }

        public CodeArrayIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
        {
            this.targetObject = targetObject;
            this.indices = new CodeExpressionCollection();
            this.indices.AddRange(indices);
        }

        public CodeExpressionCollection Indices
        {
            get
            {
                if (this.indices == null)
                {
                    this.indices = new CodeExpressionCollection();
                }
                return this.indices;
            }
        }

        public CodeExpression TargetObject
        {
            get => 
                this.targetObject;
            set
            {
                this.targetObject = value;
            }
        }
    }
}

