namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class RootNamespaceAttribute : Attribute
    {
        private string _nameSpace;

        public RootNamespaceAttribute(string nameSpace)
        {
            this._nameSpace = nameSpace;
        }

        public string Namespace =>
            this._nameSpace;
    }
}

