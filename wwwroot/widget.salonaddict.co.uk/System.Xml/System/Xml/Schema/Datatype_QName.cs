namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_QName : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(XmlQualifiedName);
        private static readonly Type listValueType = typeof(XmlQualifiedName[]);

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlMiscConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            if ((s == null) || (s.Length == 0))
            {
                return new XmlSchemaException("Sch_EmptyAttributeValue", string.Empty);
            }
            Exception exception = DatatypeImplementation.qnameFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                XmlQualifiedName name = null;
                try
                {
                    string str;
                    name = XmlQualifiedName.Parse(s, nsmgr, out str);
                }
                catch (ArgumentException exception2)
                {
                    return exception2;
                }
                catch (XmlException exception3)
                {
                    return exception3;
                }
                exception = DatatypeImplementation.qnameFacetsChecker.CheckValueFacets(name, this);
                if (exception == null)
                {
                    typedValue = name;
                    return null;
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.qnameFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.QName;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.QName;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern | RestrictionFlags.MaxLength | RestrictionFlags.MinLength | RestrictionFlags.Length);

        public override Type ValueType =>
            atomicValueType;
    }
}

