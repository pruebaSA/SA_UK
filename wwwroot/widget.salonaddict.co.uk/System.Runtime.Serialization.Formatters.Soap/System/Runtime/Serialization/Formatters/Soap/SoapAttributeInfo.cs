﻿namespace System.Runtime.Serialization.Formatters.Soap
{
    using System;
    using System.Diagnostics;

    internal sealed class SoapAttributeInfo
    {
        internal SoapAttributeType m_attributeType;
        internal string m_elementName;
        internal string m_nameSpace;
        internal string m_typeName;
        internal string m_typeNamespace;

        [Conditional("SER_LOGGING")]
        internal void Dump(string id)
        {
            this.IsXmlType();
            this.IsEmbedded();
            this.IsXmlElement();
            this.IsXmlAttribute();
        }

        internal bool IsEmbedded() => 
            ((this.m_attributeType & SoapAttributeType.Embedded) > SoapAttributeType.None);

        internal bool IsXmlAttribute() => 
            ((this.m_attributeType & SoapAttributeType.XmlAttribute) > SoapAttributeType.None);

        internal bool IsXmlElement() => 
            ((this.m_attributeType & SoapAttributeType.XmlElement) > SoapAttributeType.None);

        internal bool IsXmlType() => 
            ((this.m_attributeType & SoapAttributeType.XmlType) > SoapAttributeType.None);

        internal string AttributeElementName =>
            this.m_elementName;

        internal string AttributeTypeName =>
            this.m_typeName;
    }
}

