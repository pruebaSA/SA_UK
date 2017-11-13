namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class XmlnsDefinitionAttribute : Attribute
    {
        private string _assemblyName;
        private string _clrNamespace;
        private string _xmlNamespace;

        public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
        {
            if (xmlNamespace == null)
            {
                throw new ArgumentNullException("xmlNamespace");
            }
            if (clrNamespace == null)
            {
                throw new ArgumentNullException("clrNamespace");
            }
            this._xmlNamespace = xmlNamespace;
            this._clrNamespace = clrNamespace;
        }

        public string AssemblyName
        {
            get => 
                this._assemblyName;
            set
            {
                this._assemblyName = value;
            }
        }

        public string ClrNamespace =>
            this._clrNamespace;

        public string XmlNamespace =>
            this._xmlNamespace;
    }
}

