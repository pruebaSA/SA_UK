namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeSnippetTypeMember : CodeTypeMember
    {
        private string text;

        public CodeSnippetTypeMember()
        {
        }

        public CodeSnippetTypeMember(string text)
        {
            this.Text = text;
        }

        public string Text
        {
            get
            {
                if (this.text != null)
                {
                    return this.text;
                }
                return string.Empty;
            }
            set
            {
                this.text = value;
            }
        }
    }
}

