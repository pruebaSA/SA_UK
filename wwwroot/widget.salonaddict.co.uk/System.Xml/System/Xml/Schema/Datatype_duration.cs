namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_duration : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(TimeSpan);
        private static readonly Type listValueType = typeof(TimeSpan[]);

        internal override int Compare(object value1, object value2)
        {
            TimeSpan span = (TimeSpan) value1;
            return span.CompareTo(value2);
        }

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlMiscConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            if ((s == null) || (s.Length == 0))
            {
                return new XmlSchemaException("Sch_EmptyAttributeValue", string.Empty);
            }
            Exception exception = DatatypeImplementation.durationFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                TimeSpan span;
                exception = XmlConvert.TryToTimeSpan(s, out span);
                if (exception == null)
                {
                    exception = DatatypeImplementation.durationFacetsChecker.CheckValueFacets(span, this);
                    if (exception == null)
                    {
                        typedValue = span;
                        return null;
                    }
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.durationFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Duration;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.MinExclusive | RestrictionFlags.MinInclusive | RestrictionFlags.MaxExclusive | RestrictionFlags.MaxInclusive | RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern);

        public override Type ValueType =>
            atomicValueType;
    }
}

