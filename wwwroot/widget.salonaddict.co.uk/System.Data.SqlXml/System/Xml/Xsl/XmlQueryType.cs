namespace System.Xml.Xsl
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    internal abstract class XmlQueryType : ListBase<XmlQueryType>
    {
        private static readonly XmlTypeCode[] BaseTypeCodes;
        private int hashCode;
        private static readonly BitMatrix TypeCodeDerivation;
        private static readonly TypeFlags[] TypeCodeToFlags;
        private static readonly string[] TypeNames;

        static XmlQueryType()
        {
            TypeFlags[] flagsArray = new TypeFlags[0x37];
            flagsArray[0] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue | TypeFlags.IsNode;
            flagsArray[2] = TypeFlags.IsNode;
            flagsArray[3] = TypeFlags.IsNode;
            flagsArray[4] = TypeFlags.IsNode;
            flagsArray[5] = TypeFlags.IsNode;
            flagsArray[6] = TypeFlags.IsNode;
            flagsArray[7] = TypeFlags.IsNode;
            flagsArray[8] = TypeFlags.IsNode;
            flagsArray[9] = TypeFlags.IsNode;
            flagsArray[10] = TypeFlags.IsAtomicValue;
            flagsArray[11] = TypeFlags.IsAtomicValue;
            flagsArray[12] = TypeFlags.IsAtomicValue;
            flagsArray[13] = TypeFlags.IsAtomicValue;
            flagsArray[14] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[15] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x10] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x11] = TypeFlags.IsAtomicValue;
            flagsArray[0x12] = TypeFlags.IsAtomicValue;
            flagsArray[0x13] = TypeFlags.IsAtomicValue;
            flagsArray[20] = TypeFlags.IsAtomicValue;
            flagsArray[0x15] = TypeFlags.IsAtomicValue;
            flagsArray[0x16] = TypeFlags.IsAtomicValue;
            flagsArray[0x17] = TypeFlags.IsAtomicValue;
            flagsArray[0x18] = TypeFlags.IsAtomicValue;
            flagsArray[0x19] = TypeFlags.IsAtomicValue;
            flagsArray[0x1a] = TypeFlags.IsAtomicValue;
            flagsArray[0x1b] = TypeFlags.IsAtomicValue;
            flagsArray[0x1c] = TypeFlags.IsAtomicValue;
            flagsArray[0x1d] = TypeFlags.IsAtomicValue;
            flagsArray[30] = TypeFlags.IsAtomicValue;
            flagsArray[0x1f] = TypeFlags.IsAtomicValue;
            flagsArray[0x20] = TypeFlags.IsAtomicValue;
            flagsArray[0x21] = TypeFlags.IsAtomicValue;
            flagsArray[0x22] = TypeFlags.IsAtomicValue;
            flagsArray[0x23] = TypeFlags.IsAtomicValue;
            flagsArray[0x24] = TypeFlags.IsAtomicValue;
            flagsArray[0x25] = TypeFlags.IsAtomicValue;
            flagsArray[0x26] = TypeFlags.IsAtomicValue;
            flagsArray[0x27] = TypeFlags.IsAtomicValue;
            flagsArray[40] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x29] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2a] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2b] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2c] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2d] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2e] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x2f] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x30] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x31] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[50] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x33] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x34] = TypeFlags.IsNumeric | TypeFlags.IsAtomicValue;
            flagsArray[0x35] = TypeFlags.IsAtomicValue;
            flagsArray[0x36] = TypeFlags.IsAtomicValue;
            TypeCodeToFlags = flagsArray;
            XmlTypeCode[] codeArray = new XmlTypeCode[0x37];
            codeArray[1] = XmlTypeCode.Item;
            codeArray[2] = XmlTypeCode.Item;
            codeArray[3] = XmlTypeCode.Node;
            codeArray[4] = XmlTypeCode.Node;
            codeArray[5] = XmlTypeCode.Node;
            codeArray[6] = XmlTypeCode.Node;
            codeArray[7] = XmlTypeCode.Node;
            codeArray[8] = XmlTypeCode.Node;
            codeArray[9] = XmlTypeCode.Node;
            codeArray[10] = XmlTypeCode.Item;
            codeArray[11] = XmlTypeCode.AnyAtomicType;
            codeArray[12] = XmlTypeCode.AnyAtomicType;
            codeArray[13] = XmlTypeCode.AnyAtomicType;
            codeArray[14] = XmlTypeCode.AnyAtomicType;
            codeArray[15] = XmlTypeCode.AnyAtomicType;
            codeArray[0x10] = XmlTypeCode.AnyAtomicType;
            codeArray[0x11] = XmlTypeCode.AnyAtomicType;
            codeArray[0x12] = XmlTypeCode.AnyAtomicType;
            codeArray[0x13] = XmlTypeCode.AnyAtomicType;
            codeArray[20] = XmlTypeCode.AnyAtomicType;
            codeArray[0x15] = XmlTypeCode.AnyAtomicType;
            codeArray[0x16] = XmlTypeCode.AnyAtomicType;
            codeArray[0x17] = XmlTypeCode.AnyAtomicType;
            codeArray[0x18] = XmlTypeCode.AnyAtomicType;
            codeArray[0x19] = XmlTypeCode.AnyAtomicType;
            codeArray[0x1a] = XmlTypeCode.AnyAtomicType;
            codeArray[0x1b] = XmlTypeCode.AnyAtomicType;
            codeArray[0x1c] = XmlTypeCode.AnyAtomicType;
            codeArray[0x1d] = XmlTypeCode.AnyAtomicType;
            codeArray[30] = XmlTypeCode.AnyAtomicType;
            codeArray[0x1f] = XmlTypeCode.String;
            codeArray[0x20] = XmlTypeCode.NormalizedString;
            codeArray[0x21] = XmlTypeCode.Token;
            codeArray[0x22] = XmlTypeCode.Token;
            codeArray[0x23] = XmlTypeCode.Token;
            codeArray[0x24] = XmlTypeCode.Name;
            codeArray[0x25] = XmlTypeCode.NCName;
            codeArray[0x26] = XmlTypeCode.NCName;
            codeArray[0x27] = XmlTypeCode.NCName;
            codeArray[40] = XmlTypeCode.Decimal;
            codeArray[0x29] = XmlTypeCode.Integer;
            codeArray[0x2a] = XmlTypeCode.NonPositiveInteger;
            codeArray[0x2b] = XmlTypeCode.Integer;
            codeArray[0x2c] = XmlTypeCode.Long;
            codeArray[0x2d] = XmlTypeCode.Int;
            codeArray[0x2e] = XmlTypeCode.Short;
            codeArray[0x2f] = XmlTypeCode.Integer;
            codeArray[0x30] = XmlTypeCode.NonNegativeInteger;
            codeArray[0x31] = XmlTypeCode.UnsignedLong;
            codeArray[50] = XmlTypeCode.UnsignedInt;
            codeArray[0x33] = XmlTypeCode.UnsignedShort;
            codeArray[0x34] = XmlTypeCode.NonNegativeInteger;
            codeArray[0x35] = XmlTypeCode.Duration;
            codeArray[0x36] = XmlTypeCode.Duration;
            BaseTypeCodes = codeArray;
            TypeNames = new string[] { 
                "none", "item", "node", "document", "element", "attribute", "namespace", "processing-instruction", "comment", "text", "xdt:anyAtomicType", "xdt:untypedAtomic", "xs:string", "xs:boolean", "xs:decimal", "xs:float",
                "xs:double", "xs:duration", "xs:dateTime", "xs:time", "xs:date", "xs:gYearMonth", "xs:gYear", "xs:gMonthDay", "xs:gDay", "xs:gMonth", "xs:hexBinary", "xs:base64Binary", "xs:anyUri", "xs:QName", "xs:NOTATION", "xs:normalizedString",
                "xs:token", "xs:language", "xs:NMTOKEN", "xs:Name", "xs:NCName", "xs:ID", "xs:IDREF", "xs:ENTITY", "xs:integer", "xs:nonPositiveInteger", "xs:negativeInteger", "xs:long", "xs:int", "xs:short", "xs:byte", "xs:nonNegativeInteger",
                "xs:unsignedLong", "xs:unsignedInt", "xs:unsignedShort", "xs:unsignedByte", "xs:positiveInteger", "xdt:yearMonthDuration", "xdt:dayTimeDuration"
            };
            TypeCodeDerivation = new BitMatrix(BaseTypeCodes.Length);
            for (int i = 0; i < BaseTypeCodes.Length; i++)
            {
                int index = i;
                while (true)
                {
                    TypeCodeDerivation[i, index] = true;
                    if (BaseTypeCodes[index] == index)
                    {
                        break;
                    }
                    index = (int) BaseTypeCodes[index];
                }
            }
        }

        protected XmlQueryType()
        {
        }

        public override bool Equals(object obj)
        {
            XmlQueryType that = obj as XmlQueryType;
            if (that == null)
            {
                return false;
            }
            return this.Equals(that);
        }

        public bool Equals(XmlQueryType that)
        {
            if (that == null)
            {
                return false;
            }
            if ((this.Cardinality != that.Cardinality) || (this.IsDod != that.IsDod))
            {
                return false;
            }
            XmlQueryType prime = this.Prime;
            XmlQueryType type2 = that.Prime;
            if (prime != type2)
            {
                if (prime.Count != type2.Count)
                {
                    return false;
                }
                if (prime.Count == 1)
                {
                    return ((((prime.TypeCode == type2.TypeCode) && (prime.NameTest == type2.NameTest)) && ((prime.SchemaType == type2.SchemaType) && (prime.IsStrict == type2.IsStrict))) && (prime.IsNotRtf == type2.IsNotRtf));
                }
                foreach (XmlQueryType type3 in this)
                {
                    bool flag = false;
                    foreach (XmlQueryType type4 in that)
                    {
                        if ((((type3.TypeCode == type4.TypeCode) && (type3.NameTest == type4.NameTest)) && ((type3.SchemaType == type4.SchemaType) && (type3.IsStrict == type4.IsStrict))) && (type3.IsNotRtf == type4.IsNotRtf))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (this.hashCode == 0)
            {
                int typeCode = (int) this.TypeCode;
                XmlSchemaType schemaType = this.SchemaType;
                if (schemaType != null)
                {
                    typeCode += (typeCode << 7) ^ schemaType.GetHashCode();
                }
                typeCode += (typeCode << 7) ^ this.NodeKinds;
                typeCode += (typeCode << 7) ^ this.Cardinality.GetHashCode();
                typeCode += (typeCode << 7) ^ (this.IsStrict ? 1 : 0);
                typeCode -= typeCode >> 0x11;
                typeCode -= typeCode >> 11;
                typeCode -= typeCode >> 5;
                this.hashCode = (typeCode == 0) ? 1 : typeCode;
            }
            return this.hashCode;
        }

        public abstract void GetObjectData(BinaryWriter writer);
        private bool HasIntersectionItemType(XmlQueryType other)
        {
            if ((this.TypeCode == other.TypeCode) && ((this.NodeKinds & (XmlNodeKindFlags.Attribute | XmlNodeKindFlags.Element | XmlNodeKindFlags.Document)) != XmlNodeKindFlags.None))
            {
                if (this.TypeCode != XmlTypeCode.Node)
                {
                    if (!this.NameTest.HasIntersection(other.NameTest))
                    {
                        return false;
                    }
                    if (!XmlSchemaType.IsDerivedFrom(this.SchemaType, other.SchemaType, XmlSchemaDerivationMethod.Empty) && !XmlSchemaType.IsDerivedFrom(other.SchemaType, this.SchemaType, XmlSchemaDerivationMethod.Empty))
                    {
                        return false;
                    }
                }
                return true;
            }
            if (!this.IsSubtypeOf(other) && !other.IsSubtypeOf(this))
            {
                return false;
            }
            return true;
        }

        public bool IsSubtypeOf(XmlQueryType baseType)
        {
            if ((this.Cardinality > baseType.Cardinality) || (!this.IsDod && baseType.IsDod))
            {
                return false;
            }
            XmlQueryType prime = this.Prime;
            XmlQueryType type2 = baseType.Prime;
            if (prime != type2)
            {
                if ((prime.Count == 1) && (type2.Count == 1))
                {
                    return prime.IsSubtypeOfItemType(type2);
                }
                foreach (XmlQueryType type3 in prime)
                {
                    bool flag = false;
                    foreach (XmlQueryType type4 in type2)
                    {
                        if (type3.IsSubtypeOfItemType(type4))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsSubtypeOfItemType(XmlQueryType baseType)
        {
            XmlSchemaType schemaType = baseType.SchemaType;
            if (this.TypeCode != baseType.TypeCode)
            {
                if (baseType.IsStrict)
                {
                    return false;
                }
                XmlSchemaType builtInSimpleType = XmlSchemaType.GetBuiltInSimpleType(baseType.TypeCode);
                if ((builtInSimpleType != null) && (schemaType != builtInSimpleType))
                {
                    return false;
                }
                return TypeCodeDerivation[this.TypeCode, baseType.TypeCode];
            }
            if (baseType.IsStrict)
            {
                return (this.IsStrict && (this.SchemaType == schemaType));
            }
            if (((this.IsNotRtf || !baseType.IsNotRtf) && (this.IsDod || !baseType.IsDod)) && (this.NameTest.IsSubsetOf(baseType.NameTest) && ((schemaType == XmlSchemaComplexType.AnyType) || XmlSchemaType.IsDerivedFrom(this.SchemaType, schemaType, XmlSchemaDerivationMethod.Empty))))
            {
                if (this.IsNillable)
                {
                    return baseType.IsNillable;
                }
                return true;
            }
            return false;
        }

        private string ItemTypeToString(bool isXQ)
        {
            string str;
            if (!this.IsNode)
            {
                if (this.SchemaType != XmlSchemaComplexType.AnyType)
                {
                    if (this.SchemaType.QualifiedName.IsEmpty)
                    {
                        str = "<:" + TypeNames[(int) this.TypeCode];
                    }
                    else
                    {
                        str = QNameToString(this.SchemaType.QualifiedName);
                    }
                }
                else
                {
                    str = TypeNames[(int) this.TypeCode];
                }
            }
            else
            {
                str = TypeNames[(int) this.TypeCode];
                switch (this.TypeCode)
                {
                    case XmlTypeCode.Document:
                        if (!isXQ)
                        {
                            break;
                        }
                        str = str + "{(element" + this.NameAndType(true) + "?&text?&comment?&processing-instruction?)*}";
                        goto Label_00BA;

                    case XmlTypeCode.Element:
                    case XmlTypeCode.Attribute:
                        break;

                    default:
                        goto Label_00BA;
                }
                str = str + this.NameAndType(isXQ);
            }
        Label_00BA:
            if (!isXQ && this.IsStrict)
            {
                str = str + "=";
            }
            return str;
        }

        private string NameAndType(bool isXQ)
        {
            string str = this.NameTest.ToString();
            string str2 = "*";
            if (this.SchemaType.QualifiedName.IsEmpty)
            {
                str2 = "typeof(" + str + ")";
            }
            else if (isXQ || ((this.SchemaType != XmlSchemaComplexType.AnyType) && (this.SchemaType != DatatypeImplementation.AnySimpleType)))
            {
                str2 = QNameToString(this.SchemaType.QualifiedName);
            }
            if (this.IsNillable)
            {
                str2 = str2 + " nillable";
            }
            if ((str == "*") && (str2 == "*"))
            {
                return "";
            }
            return ("(" + str + ", " + str2 + ")");
        }

        public bool NeverSubtypeOf(XmlQueryType baseType)
        {
            if (!this.Cardinality.NeverSubset(baseType.Cardinality))
            {
                if (this.MaybeEmpty && baseType.MaybeEmpty)
                {
                    return false;
                }
                if (this.Count == 0)
                {
                    return false;
                }
                foreach (XmlQueryType type in this)
                {
                    foreach (XmlQueryType type2 in baseType)
                    {
                        if (type.HasIntersectionItemType(type2))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool operator ==(XmlQueryType left, XmlQueryType right) => 
            left?.Equals(right);

        public static bool operator !=(XmlQueryType left, XmlQueryType right)
        {
            if (left == null)
            {
                return (right != null);
            }
            return !left.Equals(right);
        }

        private static string QNameToString(XmlQualifiedName name)
        {
            if (name.IsEmpty)
            {
                return "*";
            }
            if (name.Namespace.Length == 0)
            {
                return name.Name;
            }
            if (name.Namespace == "http://www.w3.org/2001/XMLSchema")
            {
                return ("xs:" + name.Name);
            }
            if (name.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
            {
                return ("xdt:" + name.Name);
            }
            return ("{" + name.Namespace + "}" + name.Name);
        }

        public override string ToString() => 
            this.ToString("G");

        public string ToString(string format)
        {
            StringBuilder builder;
            if (format == "S")
            {
                builder = new StringBuilder();
                builder.Append(this.Cardinality.ToString(format));
                builder.Append(';');
                for (int k = 0; k < this.Count; k++)
                {
                    if (k != 0)
                    {
                        builder.Append("|");
                    }
                    builder.Append(this[k].TypeCode.ToString());
                }
                builder.Append(';');
                builder.Append(this.IsStrict);
                return builder.ToString();
            }
            bool isXQ = format == "X";
            if (this.Cardinality == XmlQueryCardinality.None)
            {
                return "none";
            }
            if (this.Cardinality == XmlQueryCardinality.Zero)
            {
                return "empty";
            }
            switch (this.Count)
            {
                case 0:
                    return ("none" + this.Cardinality.ToString());

                case 1:
                    return (this[0].ItemTypeToString(isXQ) + this.Cardinality.ToString());
            }
            string[] array = new string[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                array[i] = this[i].ItemTypeToString(isXQ);
            }
            Array.Sort<string>(array);
            builder = new StringBuilder();
            builder.Append("(");
            builder.Append(array[0]);
            for (int j = 1; j < array.Length; j++)
            {
                builder.Append(" | ");
                builder.Append(array[j]);
            }
            builder.Append(")");
            builder.Append(this.Cardinality.ToString());
            if (!isXQ && this.IsDod)
            {
                builder.Append('#');
            }
            return builder.ToString();
        }

        public abstract XmlQueryCardinality Cardinality { get; }

        public abstract XmlValueConverter ClrMapping { get; }

        public bool IsAtomicValue =>
            ((TypeCodeToFlags[(int) this.TypeCode] & TypeFlags.IsAtomicValue) != TypeFlags.None);

        public abstract bool IsDod { get; }

        public bool IsEmpty =>
            (this.Cardinality <= XmlQueryCardinality.Zero);

        public abstract bool IsNillable { get; }

        public bool IsNode =>
            ((TypeCodeToFlags[(int) this.TypeCode] & TypeFlags.IsNode) != TypeFlags.None);

        public abstract bool IsNotRtf { get; }

        public bool IsNumeric =>
            ((TypeCodeToFlags[(int) this.TypeCode] & TypeFlags.IsNumeric) != TypeFlags.None);

        public bool IsSingleton =>
            (this.Cardinality <= XmlQueryCardinality.One);

        public abstract bool IsStrict { get; }

        public bool MaybeEmpty =>
            (XmlQueryCardinality.Zero <= this.Cardinality);

        public bool MaybeMany =>
            (XmlQueryCardinality.More <= this.Cardinality);

        public abstract XmlQualifiedNameTest NameTest { get; }

        public abstract XmlNodeKindFlags NodeKinds { get; }

        public abstract XmlQueryType Prime { get; }

        public abstract XmlSchemaType SchemaType { get; }

        public abstract XmlTypeCode TypeCode { get; }

        private sealed class BitMatrix
        {
            private ulong[] bits;

            public BitMatrix(int count)
            {
                this.bits = new ulong[count];
            }

            public bool this[int index1, int index2]
            {
                get => 
                    ((this.bits[index1] & (((ulong) 1L) << index2)) != 0L);
                set
                {
                    if (value)
                    {
                        this.bits[index1] |= ((ulong) 1L) << index2;
                    }
                    else
                    {
                        this.bits[index1] &= (ulong) ~(((long) 1L) << index2);
                    }
                }
            }

            public bool this[XmlTypeCode index1, XmlTypeCode index2] =>
                this[(int) index1, (int) index2];
        }

        private enum TypeFlags
        {
            IsAtomicValue = 2,
            IsNode = 1,
            IsNumeric = 4,
            None = 0
        }
    }
}

