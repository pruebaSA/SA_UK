﻿namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaComplexContentRestriction : XmlSchemaContent
    {
        private XmlSchemaAnyAttribute anyAttribute;
        private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();
        private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;
        private XmlSchemaParticle particle;

        internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
        {
            this.attributes = newAttributes;
        }

        [XmlElement("anyAttribute")]
        public XmlSchemaAnyAttribute AnyAttribute
        {
            get => 
                this.anyAttribute;
            set
            {
                this.anyAttribute = value;
            }
        }

        [XmlElement("attribute", typeof(XmlSchemaAttribute)), XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
        public XmlSchemaObjectCollection Attributes =>
            this.attributes;

        [XmlAttribute("base")]
        public XmlQualifiedName BaseTypeName
        {
            get => 
                this.baseTypeName;
            set
            {
                this.baseTypeName = (value == null) ? XmlQualifiedName.Empty : value;
            }
        }

        [XmlElement("sequence", typeof(XmlSchemaSequence)), XmlElement("group", typeof(XmlSchemaGroupRef)), XmlElement("choice", typeof(XmlSchemaChoice)), XmlElement("all", typeof(XmlSchemaAll))]
        public XmlSchemaParticle Particle
        {
            get => 
                this.particle;
            set
            {
                this.particle = value;
            }
        }
    }
}
