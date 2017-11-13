namespace System.Windows.Markup
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class XmlnsPrefixAttribute : Attribute
    {
        private string _prefix;
        private string _xmlNamespace;

        public XmlnsPrefixAttribute(string xmlNamespace, string prefix)
        {
            if (xmlNamespace == null)
            {
                throw new ArgumentNullException("xmlNamespace");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            this._xmlNamespace = xmlNamespace;
            this._prefix = prefix;
        }

        public string Prefix =>
            this._prefix;

        public string XmlNamespace =>
            this._xmlNamespace;
    }
}

