namespace System.CodeDom
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true)]
    public class CodeNamespaceImport : CodeObject
    {
        private CodeLinePragma linePragma;
        private string nameSpace;

        public CodeNamespaceImport()
        {
        }

        public CodeNamespaceImport(string nameSpace)
        {
            this.Namespace = nameSpace;
        }

        public CodeLinePragma LinePragma
        {
            get => 
                this.linePragma;
            set
            {
                this.linePragma = value;
            }
        }

        public string Namespace
        {
            get
            {
                if (this.nameSpace != null)
                {
                    return this.nameSpace;
                }
                return string.Empty;
            }
            set
            {
                this.nameSpace = value;
            }
        }
    }
}

