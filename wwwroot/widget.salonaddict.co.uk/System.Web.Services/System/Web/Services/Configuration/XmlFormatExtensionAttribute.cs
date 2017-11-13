namespace System.Web.Services.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionAttribute : Attribute
    {
        private string name;
        private string ns;
        private Type[] types;

        public XmlFormatExtensionAttribute()
        {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1) : this(elementName, ns, new Type[] { extensionPoint1 })
        {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type[] extensionPoints)
        {
            this.name = elementName;
            this.ns = ns;
            this.types = extensionPoints;
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2 })
        {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3 })
        {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3, Type extensionPoint4) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3, extensionPoint4 })
        {
        }

        public string ElementName
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }
                return string.Empty;
            }
            set
            {
                this.name = value;
            }
        }

        public Type[] ExtensionPoints
        {
            get
            {
                if (this.types != null)
                {
                    return this.types;
                }
                return new Type[0];
            }
            set
            {
                this.types = value;
            }
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
    }
}

