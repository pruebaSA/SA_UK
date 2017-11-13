namespace System.Web.Services.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class XmlFormatExtensionPrefixAttribute : Attribute
    {
        private string ns;
        private string prefix;

        public XmlFormatExtensionPrefixAttribute()
        {
        }

        public XmlFormatExtensionPrefixAttribute(string prefix, string ns)
        {
            this.prefix = prefix;
            this.ns = ns;
        }

        public string Namespace
        {
            get
            {
                if (this.ns != null)
                {
                    return this.ns;
                }
                return string.Empty;
            }
            set
            {
                this.ns = value;
            }
        }

        public string Prefix
        {
            get
            {
                if (this.prefix != null)
                {
                    return this.prefix;
                }
                return string.Empty;
            }
            set
            {
                this.prefix = value;
            }
        }
    }
}

