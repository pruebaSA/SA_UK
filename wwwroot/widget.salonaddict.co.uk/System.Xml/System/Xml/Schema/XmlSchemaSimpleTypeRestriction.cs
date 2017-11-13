namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaSimpleTypeRestriction : XmlSchemaSimpleTypeContent
    {
        private XmlSchemaSimpleType baseType;
        private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;
        private XmlSchemaObjectCollection facets = new XmlSchemaObjectCollection();

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) base.MemberwiseClone();
            restriction.BaseTypeName = this.baseTypeName.Clone();
            return restriction;
        }

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

        [XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet)), XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet)), XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet)), XmlElement("length", typeof(XmlSchemaLengthFacet)), XmlElement("minLength", typeof(XmlSchemaMinLengthFacet)), XmlElement("pattern", typeof(XmlSchemaPatternFacet)), XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet)), XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet)), XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet)), XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet)), XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet)), XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
        public XmlSchemaObjectCollection Facets =>
            this.facets;
    }
}

