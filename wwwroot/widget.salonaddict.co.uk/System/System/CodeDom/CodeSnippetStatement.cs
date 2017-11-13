namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CodeSnippetStatement : CodeStatement
    {
        private string value;

        public CodeSnippetStatement()
        {
        }

        public CodeSnippetStatement(string value)
        {
            this.Value = value;
        }

        public string Value
        {
            get
            {
                if (this.value != null)
                {
                    return this.value;
                }
                return string.Empty;
            }
            set
            {
                this.value = value;
            }
        }
    }
}

