namespace System.Xml.Schema
{
    using System;

    internal class XsdSimpleValue
    {
        private object typedValue;
        private XmlSchemaSimpleType xmlType;

        public XsdSimpleValue(XmlSchemaSimpleType st, object value)
        {
            this.xmlType = st;
            this.typedValue = value;
        }

        public object TypedValue =>
            this.typedValue;

        public XmlSchemaSimpleType XmlType =>
            this.xmlType;
    }
}

