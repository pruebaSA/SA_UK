namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaGroup : XmlSchemaAnnotated
    {
        private XmlSchemaParticle canonicalParticle;
        private string name;
        private XmlSchemaGroupBase particle;
        private XmlQualifiedName qname = XmlQualifiedName.Empty;
        private XmlSchemaGroup redefined;
        private int selfReferenceCount;

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaGroup group = (XmlSchemaGroup) base.MemberwiseClone();
            if (XmlSchemaComplexType.HasParticleRef(this.particle))
            {
                group.particle = XmlSchemaComplexType.CloneParticle(this.particle) as XmlSchemaGroupBase;
            }
            group.canonicalParticle = XmlSchemaParticle.Empty;
            return group;
        }

        internal void SetQualifiedName(XmlQualifiedName value)
        {
            this.qname = value;
        }

        [XmlIgnore]
        internal XmlSchemaParticle CanonicalParticle
        {
            get => 
                this.canonicalParticle;
            set
            {
                this.canonicalParticle = value;
            }
        }

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

        [XmlElement("all", typeof(XmlSchemaAll)), XmlElement("sequence", typeof(XmlSchemaSequence)), XmlElement("choice", typeof(XmlSchemaChoice))]
        public XmlSchemaGroupBase Particle
        {
            get => 
                this.particle;
            set
            {
                this.particle = value;
            }
        }

        [XmlIgnore]
        public XmlQualifiedName QualifiedName =>
            this.qname;

        [XmlIgnore]
        internal XmlSchemaGroup Redefined
        {
            get => 
                this.redefined;
            set
            {
                this.redefined = value;
            }
        }

        [XmlIgnore]
        internal int SelfReferenceCount
        {
            get => 
                this.selfReferenceCount;
            set
            {
                this.selfReferenceCount = value;
            }
        }
    }
}

