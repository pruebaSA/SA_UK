namespace System.Xml.Schema
{
    using System;
    using System.Globalization;

    internal class TypedObject
    {
        private int dim = 1;
        private DecimalStruct dstruct;
        private bool isList;
        private object ovalue;
        private string svalue;
        private XmlSchemaDatatype xsdtype;

        public TypedObject(object obj, string svalue, XmlSchemaDatatype xsdtype)
        {
            this.ovalue = obj;
            this.svalue = svalue;
            this.xsdtype = xsdtype;
            if (((xsdtype.Variety == XmlSchemaDatatypeVariety.List) || (xsdtype is Datatype_base64Binary)) || (xsdtype is Datatype_hexBinary))
            {
                this.isList = true;
                this.dim = ((Array) obj).Length;
            }
        }

        public bool Equals(TypedObject other)
        {
            if (this.Dim != other.Dim)
            {
                return false;
            }
            if (this.Type != other.Type)
            {
                if (!this.Type.IsComparable(other.Type))
                {
                    return false;
                }
                other.SetDecimal();
                this.SetDecimal();
                if (this.IsDecimal && other.IsDecimal)
                {
                    return this.ListDValueEquals(other);
                }
            }
            if (this.IsList)
            {
                if (other.IsList)
                {
                    return (this.Type.Compare(this.Value, other.Value) == 0);
                }
                Array array = this.Value as Array;
                XmlAtomicValue[] valueArray = array as XmlAtomicValue[];
                if (valueArray != null)
                {
                    return ((valueArray.Length == 1) && valueArray.GetValue(0).Equals(other.Value));
                }
                return ((array.Length == 1) && array.GetValue(0).Equals(other.Value));
            }
            if (!other.IsList)
            {
                return this.Value.Equals(other.Value);
            }
            Array array2 = other.Value as Array;
            XmlAtomicValue[] valueArray2 = array2 as XmlAtomicValue[];
            if (valueArray2 != null)
            {
                return ((valueArray2.Length == 1) && valueArray2.GetValue(0).Equals(this.Value));
            }
            return ((array2.Length == 1) && array2.GetValue(0).Equals(this.Value));
        }

        private bool ListDValueEquals(TypedObject other)
        {
            for (int i = 0; i < this.Dim; i++)
            {
                if (this.Dvalue[i] != other.Dvalue[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void SetDecimal()
        {
            if (this.dstruct == null)
            {
                switch (this.xsdtype.TypeCode)
                {
                    case XmlTypeCode.Integer:
                    case XmlTypeCode.NonPositiveInteger:
                    case XmlTypeCode.NegativeInteger:
                    case XmlTypeCode.Long:
                    case XmlTypeCode.Int:
                    case XmlTypeCode.Short:
                    case XmlTypeCode.Byte:
                    case XmlTypeCode.NonNegativeInteger:
                    case XmlTypeCode.UnsignedLong:
                    case XmlTypeCode.UnsignedInt:
                    case XmlTypeCode.UnsignedShort:
                    case XmlTypeCode.UnsignedByte:
                    case XmlTypeCode.PositiveInteger:
                    case XmlTypeCode.Decimal:
                        if (!this.isList)
                        {
                            this.dstruct = new DecimalStruct();
                            this.dstruct.Dvalue[0] = Convert.ToDecimal(this.ovalue, NumberFormatInfo.InvariantInfo);
                            break;
                        }
                        this.dstruct = new DecimalStruct(this.dim);
                        for (int i = 0; i < this.dim; i++)
                        {
                            this.dstruct.Dvalue[i] = Convert.ToDecimal(((Array) this.ovalue).GetValue(i), NumberFormatInfo.InvariantInfo);
                        }
                        break;

                    default:
                        if (this.isList)
                        {
                            this.dstruct = new DecimalStruct(this.dim);
                        }
                        else
                        {
                            this.dstruct = new DecimalStruct();
                        }
                        return;
                }
                this.dstruct.IsDecimal = true;
            }
        }

        public override string ToString() => 
            this.svalue;

        public int Dim =>
            this.dim;

        public decimal[] Dvalue =>
            this.dstruct.Dvalue;

        public bool IsDecimal =>
            this.dstruct.IsDecimal;

        public bool IsList =>
            this.isList;

        public XmlSchemaDatatype Type
        {
            get => 
                this.xsdtype;
            set
            {
                this.xsdtype = value;
            }
        }

        public object Value
        {
            get => 
                this.ovalue;
            set
            {
                this.ovalue = value;
            }
        }

        private class DecimalStruct
        {
            private decimal[] dvalue;
            private bool isDecimal;

            public DecimalStruct()
            {
                this.dvalue = new decimal[1];
            }

            public DecimalStruct(int dim)
            {
                this.dvalue = new decimal[dim];
            }

            public decimal[] Dvalue =>
                this.dvalue;

            public bool IsDecimal
            {
                get => 
                    this.isDecimal;
                set
                {
                    this.isDecimal = value;
                }
            }
        }
    }
}

