﻿namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_base64Binary : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(byte[]);
        private static readonly Type listValueType = typeof(byte[][]);

        internal override int Compare(object value1, object value2) => 
            base.Compare((byte[]) value1, (byte[]) value2);

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlMiscConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = DatatypeImplementation.binaryFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                byte[] buffer = null;
                try
                {
                    buffer = Convert.FromBase64String(s);
                }
                catch (ArgumentException exception2)
                {
                    return exception2;
                }
                catch (FormatException exception3)
                {
                    return exception3;
                }
                exception = DatatypeImplementation.binaryFacetsChecker.CheckValueFacets(buffer, this);
                if (exception == null)
                {
                    typedValue = buffer;
                    return null;
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.binaryFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Base64Binary;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.WhiteSpace | RestrictionFlags.Enumeration | RestrictionFlags.Pattern | RestrictionFlags.MaxLength | RestrictionFlags.MinLength | RestrictionFlags.Length);

        public override Type ValueType =>
            atomicValueType;
    }
}

