namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaGroupRef : XmlSchemaParticle
    {
        private XmlSchemaGroupBase particle;
        private XmlSchemaGroup refined;
        private XmlQualifiedName refName = XmlQualifiedName.Empty;

        internal void SetParticle(XmlSchemaGroupBase value)
        {
            this.particle = value;
        }

        [XmlIgnore]
        public XmlSchemaGroupBase Particle =>
            this.particle;

        [XmlIgnore]
        internal XmlSchemaGroup Redefined
        {
            get => 
                this.refined;
            set
            {
                this.refined = value;
            }
        }

        [XmlAttribute("ref")]
        public XmlQualifiedName RefName
        {
            get => 
                this.refName;
            set
            {
                this.refName = (value == null) ? XmlQualifiedName.Empty : value;
            }
        }
    }
}

