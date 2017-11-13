﻿namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_boolean : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(bool);
        private static readonly Type listValueType = typeof(bool[]);

        internal override int Compare(object value1, object value2)
        {
            bool flag = (bool) value1;
            return flag.CompareTo(value2);
        }

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlBooleanConverter.Create(schemaType);

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = DatatypeImplementation.miscFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                bool flag;
                exception = XmlConvert.TryToBoolean(s, out flag);
                if (exception == null)
                {
                    typedValue = flag;
                    return null;
                }
            }
            return exception;
        }

        internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet =>
            XmlSchemaWhiteSpace.Collapse;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.miscFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Boolean;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.WhiteSpace | RestrictionFlags.Pattern);

        public override Type ValueType =>
            atomicValueType;
    }
}

