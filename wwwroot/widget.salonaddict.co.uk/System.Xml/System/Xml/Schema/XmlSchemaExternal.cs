namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class XmlSchemaExternal : XmlSchemaObject
    {
        private Uri baseUri;
        private System.Xml.Schema.Compositor compositor;
        private string id;
        private string location;
        private XmlAttribute[] moreAttributes;
        private XmlSchema schema;

        protected XmlSchemaExternal()
        {
        }

        internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
        {
            this.moreAttributes = moreAttributes;
        }

        [XmlIgnore]
        internal Uri BaseUri
        {
            get => 
                this.baseUri;
            set
            {
                this.baseUri = value;
            }
        }

        internal System.Xml.Schema.Compositor Compositor
        {
            get => 
                this.compositor;
            set
            {
                this.compositor = value;
            }
        }

        [XmlAttribute("id", DataType="ID")]
        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        [XmlIgnore]
        internal override string IdAttribute
        {
            get => 
                this.Id;
            set
            {
                this.Id = value;
            }
        }

        [XmlIgnore]
        public XmlSchema Schema
        {
            get => 
                this.schema;
            set
            {
                this.schema = value;
            }
        }

        [XmlAttribute("schemaLocation", DataType="anyURI")]
        public string SchemaLocation
        {
            get => 
                this.location;
            set
            {
                this.location = value;
            }
        }

        [XmlAnyAttribute]
        public XmlAttribute[] UnhandledAttributes
        {
            get => 
                this.moreAttributes;
            set
            {
                this.moreAttributes = value;
            }
        }
    }
}

