﻿namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_anyURI : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(Uri);
        private static readonly Type listValueType = typeof(Uri[]);

        internal override int Compare(object value1, object value2)
        {
            if (!((Uri) value1).Equals((Uri) value2))
            {
                return -1;
            }
            return 0;
        }

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlMiscConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = DatatypeImplementation.stringFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                Uri uri;
                exception = XmlConvert.TryToUri(s, out uri);
                if (exception == null)
                {
                    string originalString = uri.OriginalString;
                    exception = ((StringFacetsChecker) DatatypeImplementation.stringFacetsChecker).CheckValueFacets(originalString, this, false);
                    if (exception == null)
                    {
                        typedValue = uri;
                        return null;
                    }
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.stringFacetsChecker;

        internal override bool HasValueFacets =>
            true;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.AnyUri;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern | RestrictionFlags.MaxLength | RestrictionFlags.MinLength | RestrictionFlags.Length);

        public override Type ValueType =>
            atomicValueType;
    }
}

