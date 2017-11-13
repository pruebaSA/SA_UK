namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class MessagePart : NamedItem
    {
        private XmlQualifiedName element = XmlQualifiedName.Empty;
        private ServiceDescriptionFormatExtensionCollection extensions;
        private System.Web.Services.Description.Message parent;
        private XmlQualifiedName type = XmlQualifiedName.Empty;

        internal void SetParent(System.Web.Services.Description.Message parent)
        {
            this.parent = parent;
        }

        [XmlAttribute("element")]
        public XmlQualifiedName Element
        {
            get => 
                this.element;
            set
            {
                this.element = value;
            }
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get
            {
                if (this.extensions == null)
                {
                    this.extensions = new ServiceDescriptionFormatExtensionCollection(this);
                }
                return this.extensions;
            }
        }

        public System.Web.Services.Description.Message Message =>
            this.parent;

        [XmlAttribute("type")]
        public XmlQualifiedName Type
        {
            get
            {
                if (this.type == null)
                {
                    return XmlQualifiedName.Empty;
                }
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}

