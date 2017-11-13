namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_anySimpleType : DatatypeImplementation
    {
        private static readonly Type atomicValueType = typeof(string);
        private static readonly Type listValueType = typeof(string[]);

        internal override int Compare(object value1, object value2) => 
            string.Compare(value1.ToString(), value2.ToString(), StringComparison.Ordinal);

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlUntypedConverter.Untyped;

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = XmlComplianceUtil.NonCDataNormalize(s);
            return null;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.miscFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.None;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.AnyAtomicType;

        internal override RestrictionFlags ValidRestrictionFlags =>
            0;

        public override Type ValueType =>
            atomicValueType;
    }
}

