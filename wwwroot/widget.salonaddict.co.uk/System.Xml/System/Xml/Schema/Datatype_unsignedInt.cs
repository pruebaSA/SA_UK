namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_unsignedInt : Datatype_unsignedLong
    {
        private static readonly Type atomicValueType = typeof(uint);
        private static readonly Type listValueType = typeof(uint[]);
        private static readonly System.Xml.Schema.FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(0M, 4294967295M);

        internal override int Compare(object value1, object value2)
        {
            uint num = (uint) value1;
            return num.CompareTo(value2);
        }

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            typedValue = null;
            Exception exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                uint num;
                exception = XmlConvert.TryToUInt32(s, out num);
                if (exception == null)
                {
                    exception = numeric10FacetsChecker.CheckValueFacets((long) num, this);
                    if (exception == null)
                    {
                        typedValue = num;
                        return null;
                    }
                }
            }
            return exception;
        }

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            numeric10FacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.UnsignedInt;

        public override Type ValueType =>
            atomicValueType;
    }
}

