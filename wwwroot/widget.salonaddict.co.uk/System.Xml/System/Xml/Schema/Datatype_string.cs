﻿namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_string : Datatype_anySimpleType
    {
        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlStringConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = DatatypeImplementation.stringFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                exception = DatatypeImplementation.stringFacetsChecker.CheckValueFacets(s, this);
                if (exception == null)
                {
                    typedValue = s;
                    return null;
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Preserve;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.stringFacetsChecker;

        public override XmlTokenizedType TokenizedType =>
            XmlTokenizedType.CDATA;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.String;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern | RestrictionFlags.MaxLength | RestrictionFlags.MinLength | RestrictionFlags.Length);
    }
}

