namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaSimpleContentRestriction : XmlSchemaContent
    {
        private XmlSchemaAnyAttribute anyAttribute;
        private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();
        private XmlSchemaSimpleType baseType;
        private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;
        private XmlSchemaObjectCollection facets = new XmlSchemaObjectCollection();

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

        [XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
        public XmlSchemaSimpleType BaseType
        {
            get => 
                this.baseType;
            set
            {
                this.baseType = value;
            }
        }

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

        [XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet)), XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet)), XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet)), XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet)), XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet)), XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet)), XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet)), XmlElement("length", typeof(XmlSchemaLengthFacet)), XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet)), XmlElement("minLength", typeof(XmlSchemaMinLengthFacet)), XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet)), XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
        public XmlSchemaObjectCollection Facets =>
            this.facets;
    }
}

