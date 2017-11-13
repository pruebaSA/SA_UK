namespace System.Xml.Schema
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class Datatype_union : Datatype_anySimpleType
    {
        private static readonly Type atomicValueType = typeof(object);
        private static readonly Type listValueType = typeof(object[]);
        private XmlSchemaSimpleType[] types;

        internal Datatype_union(XmlSchemaSimpleType[] types)
        {
            this.types = types;
        }

        internal override int Compare(object value1, object value2)
        {
            XsdSimpleValue value3 = value1 as XsdSimpleValue;
            XsdSimpleValue value4 = value2 as XsdSimpleValue;
            if ((value3 != null) && (value4 != null))
            {
                XmlSchemaType xmlType = value3.XmlType;
                XmlSchemaType type2 = value4.XmlType;
                if (xmlType == type2)
                {
                    return xmlType.Datatype.Compare(value3.TypedValue, value4.TypedValue);
                }
            }
            return -1;
        }

        internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) => 
            XmlUnionConverter.Create(schemaType);

        internal bool HasAtomicMembers()
        {
            foreach (XmlSchemaSimpleType type in this.types)
            {
                if (type.Datatype.Variety == XmlSchemaDatatypeVariety.List)
                {
                    return false;
                }
            }
            return true;
        }

        internal bool IsUnionBaseOf(DatatypeImplementation derivedType)
        {
            foreach (XmlSchemaSimpleType type in this.types)
            {
                if (derivedType.IsDerivedFrom(type.Datatype))
                {
                    return true;
                }
            }
            return false;
        }

        internal override Exception TryParseValue(object value, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            Exception exception;
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            typedValue = null;
            string s = value as string;
            if (s != null)
            {
                return this.TryParseValue(s, nameTable, nsmgr, out typedValue);
            }
            object obj2 = null;
            XmlSchemaSimpleType st = null;
            foreach (XmlSchemaSimpleType type2 in this.types)
            {
                if (type2.Datatype.TryParseValue(value, nameTable, nsmgr, out obj2) == null)
                {
                    st = type2;
                    break;
                }
            }
            if (obj2 == null)
            {
                return new XmlSchemaException("Sch_UnionFailedEx", value.ToString());
            }
            try
            {
                if (this.HasLexicalFacets)
                {
                    string parseString = (string) this.ValueConverter.ChangeType(obj2, typeof(string), nsmgr);
                    exception = DatatypeImplementation.unionFacetsChecker.CheckLexicalFacets(ref parseString, this);
                    if (exception != null)
                    {
                        return exception;
                    }
                }
                typedValue = new XsdSimpleValue(st, obj2);
                if (this.HasValueFacets)
                {
                    exception = DatatypeImplementation.unionFacetsChecker.CheckValueFacets(typedValue, this);
                    if (exception != null)
                    {
                        return exception;
                    }
                }
                return null;
            }
            catch (FormatException exception2)
            {
                exception = exception2;
            }
            catch (InvalidCastException exception3)
            {
                exception = exception3;
            }
            catch (OverflowException exception4)
            {
                exception = exception4;
            }
            catch (ArgumentException exception5)
            {
                exception = exception5;
            }
            return exception;
        }

        internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
        {
            XmlSchemaSimpleType st = null;
            typedValue = null;
            Exception exception = DatatypeImplementation.unionFacetsChecker.CheckLexicalFacets(ref s, this);
            if (exception == null)
            {
                foreach (XmlSchemaSimpleType type2 in this.types)
                {
                    if (type2.Datatype.TryParseValue(s, nameTable, nsmgr, out typedValue) == null)
                    {
                        st = type2;
                        break;
                    }
                }
                if (st == null)
                {
                    return new XmlSchemaException("Sch_UnionFailedEx", s);
                }
                typedValue = new XsdSimpleValue(st, typedValue);
                exception = DatatypeImplementation.unionFacetsChecker.CheckValueFacets(typedValue, this);
                if (exception == null)
                {
                    return null;
                }
            }
            return exception;
        }

        internal XmlSchemaSimpleType[] BaseMemberTypes =>
            this.types;

        internal override System.Xml.Schema.FacetsChecker FacetsChecker =>
            DatatypeImplementation.unionFacetsChecker;

        internal override Type ListValueType =>
            listValueType;

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.AnyAtomicType;

        internal override RestrictionFlags ValidRestrictionFlags =>
            (RestrictionFlags.Enumeration | RestrictionFlags.Pattern);

        public override Type ValueType =>
            atomicValueType;
    }
}

