namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_double : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(double);
        private static readonly Type listValueType = typeof(double[]);

        internal override int Compare(object value1, object value2)
        {
            double num = (double) value1;
            return num.CompareTo(value2);
        }

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlNumeric2Converter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = DatatypeImplementation.numeric2FacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                double num;
                exception = XmlConvert.TryToDouble(s, out num);
                if (exception == null)
                {
                    exception = DatatypeImplementation.numeric2FacetsChecker.CheckValueFacets(num, this);
                    if (exception == null)
                    {
                        typedValue = num;
                        return null;
                    }
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.numeric2FacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Double;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.MinExclusive | RestrictionFlags.MinInclusive | RestrictionFlags.MaxExclusive | RestrictionFlags.MaxInclusive | RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern);

        public override Type ValueType =>
            atomicValueType;
    }
}

