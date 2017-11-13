namespace System.Web.Services.Description
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class DocumentableItem
    {
        private XmlAttribute[] anyAttribute;
        private string documentation;
        private XmlElement documentationElement;
        private XmlSerializerNamespaces namespaces;
        private XmlDocument parent;

        protected DocumentableItem()
        {
        }

        internal XmlElement GetDocumentationElement()
        {
            if (this.documentationElement == null)
            {
                this.documentationElement = this.Parent.CreateElement("wsdl", "documentation", "http://schemas.xmlsoap.org/wsdl/");
                this.Parent.InsertBefore(this.documentationElement, null);
            }
            return this.documentationElement;
        }

        [XmlIgnore]
        public string Documentation
        {
            get
            {
                if (this.documentation != null)
                {
                    return this.documentation;
                }
                return this.documentationElement?.InnerXml;
            }
            set
            {
                this.documentation = value;
                StringWriter w = new StringWriter(CultureInfo.InvariantCulture);
                XmlTextWriter writer2 = new XmlTextWriter(w);
                writer2.WriteElementString("wsdl", "documentation", "http://schemas.xmlsoap.org/wsdl/", value);
                this.Parent.LoadXml(w.ToString());
                this.documentationElement = this.parent.DocumentElement;
                writer2.Close();
            }
        }

        [ComVisible(false), XmlAnyElement("documentation", Namespace="http://schemas.xmlsoap.org/wsdl/")]
        public XmlElement DocumentationElement
        {
            get => 
                this.documentationElement;
            set
            {
                this.documentationElement = value;
                this.documentation = null;
            }
        }

        [XmlAnyAttribute]
        public XmlAttribute[] ExtensibleAttributes
        {
            get => 
                this.anyAttribute;
            set
            {
                this.anyAttribute = value;
            }
        }

        [XmlIgnore]
        public abstract ServiceDescriptionFormatExtensionCollection Extensions { get; }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces
        {
            get
            {
                if (this.namespaces == null)
                {
                    this.namespaces = new XmlSerializerNamespaces();
                }
                return this.namespaces;
            }
            set
            {
                this.namespaces = value;
            }
        }

        internal XmlDocument Parent
        {
            get
            {
                if (this.parent == null)
                {
                    this.parent = new XmlDocument();
                }
                return this.parent;
            }
        }
    }
}

