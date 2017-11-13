namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Port : NamedItem
    {
        private XmlQualifiedName binding = XmlQualifiedName.Empty;
        private ServiceDescriptionFormatExtensionCollection extensions;
        private System.Web.Services.Description.Service parent;

        internal void SetParent(System.Web.Services.Description.Service parent)
        {
            this.parent = parent;
        }

        [XmlAttribute("binding")]
        public XmlQualifiedName Binding
        {
            get => 
                this.binding;
            set
            {
                this.binding = value;
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

        public System.Web.Services.Description.Service Service =>
            this.parent;
    }
}

