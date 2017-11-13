namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaNotation : XmlSchemaAnnotated
    {
        private string name;
        private string publicId;
        private XmlQualifiedName qname = XmlQualifiedName.Empty;
        private string systemId;

        [XmlAttribute("name")]
        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        [XmlIgnore]
        internal override string NameAttribute
        {
            get => 
                this.Name;
            set
            {
                this.Name = value;
            }
        }

        [XmlAttribute("public")]
        public string Public
        {
            get => 
                this.publicId;
            set
            {
                this.publicId = value;
            }
        }

        [XmlIgnore]
        internal XmlQualifiedName QualifiedName
        {
            get => 
                this.qname;
            set
            {
                this.qname = value;
            }
        }

        [XmlAttribute("system")]
        public string System
        {
            get => 
                this.systemId;
            set
            {
                this.systemId = value;
            }
        }
    }
}

