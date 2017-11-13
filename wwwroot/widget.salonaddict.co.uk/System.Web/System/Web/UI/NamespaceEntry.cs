namespace System.Web.UI
{
    using System;

    internal class NamespaceEntry : SourceLineInfo
    {
        private string _namespace;

        internal NamespaceEntry()
        {
        }

        internal string Namespace
        {
            get => 
                this._namespace;
            set
            {
                this._namespace = value;
            }
        }
    }
}

