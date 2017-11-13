namespace System.Xml.Xsl
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XmlQueryCardinality
    {
        private int value;
        private static readonly XmlQueryCardinality[,] cardinalityProduct;
        private static readonly XmlQueryCardinality[,] cardinalitySum;
        private static readonly string[] toString;
        private static readonly string[] serialized;
        private XmlQueryCardinality(int value)
        {
            this.value = value;
        }

        public static XmlQueryCardinality None =>
            new XmlQueryCardinality(0);
        public static XmlQueryCardinality Zero =>
            new XmlQueryCardinality(1);
        public static XmlQueryCardinality One =>
            new XmlQueryCardinality(2);
        public static XmlQueryCardinality ZeroOrOne =>
            new XmlQueryCardinality(3);
        public static XmlQueryCardinality More =>
            new XmlQueryCardinality(4);
        public static XmlQueryCardinality NotOne =>
            new XmlQueryCardinality(5);
        public static XmlQueryCardinality OneOrMore =>
            new XmlQueryCardinality(6);
        public static XmlQueryCardinality ZeroOrMore =>
            new XmlQueryCardinality(7);
        public bool Equals(XmlQueryCardinality other) => 
            (this.value == other.value);

        public static bool operator ==(XmlQueryCardinality left, XmlQueryCardinality right) => 
            (left.value == right.value);

        public static bool operator !=(XmlQueryCardinality left, XmlQueryCardinality right) => 
            (left.value != right.value);

        public override bool Equals(object other) => 
            ((other is XmlQueryCardinality) && this.Equals((XmlQueryCardinality) other));

        public override int GetHashCode() => 
            this.value;

        public static XmlQueryCardinality operator |(XmlQueryCardinality left, XmlQueryCardinality right) => 
            new XmlQueryCardinality(left.value | right.value);

        public static XmlQueryCardinality operator &(XmlQueryCardinality left, XmlQueryCardinality right) => 
            new XmlQueryCardinality(left.value & right.value);

        public static XmlQueryCardinality operator *(XmlQueryCardinality left, XmlQueryCardinality right) => 
            *(cardinalityProduct[left.value, right.value]);

        public static XmlQueryCardinality operator +(XmlQueryCardinality left, XmlQueryCardinality right) => 
            *(cardinalitySum[left.value, right.value]);

        public static bool operator <=(XmlQueryCardinality left, XmlQueryCardinality right) => 
            ((left.value & ~right.value) == 0);

        public static bool operator >=(XmlQueryCardinality left, XmlQueryCardinality right) => 
            ((right.value & ~left.value) == 0);

        public XmlQueryCardinality AtMost() => 
            new XmlQueryCardinality((this.value | (this.value >> 1)) | (this.value >> 2));

        public bool NeverSubset(XmlQueryCardinality other) => 
            ((this.value != 0) && ((this.value & other.value) == 0));

        public string ToString(string format)
        {
            if (format == "S")
            {
                return serialized[this.value];
            }
            return this.ToString();
        }

        public override string ToString() => 
            toString[this.value];

        public XmlQueryCardinality(string s)
        {
            this.value = 0;
            for (int i = 0; i < serialized.Length; i++)
            {
                if (s == serialized[i])
                {
                    this.value = i;
                    return;
                }
            }
        }

        public void GetObjectData(BinaryWriter writer)
        {
            writer.Write((byte) this.value);
        }

        public XmlQueryCardinality(BinaryReader reader) : this(reader.ReadByte())
        {
        }

        static XmlQueryCardinality()
        {
            XmlQueryCardinality[,] cardinalityArray = new XmlQueryCardinality[8, 8];
            *(cardinalityArray[0, 0]) = None;
            *(cardinalityArray[0, 1]) = Zero;
            *(cardinalityArray[0, 2]) = None;
            *(cardinalityArray[0, 3]) = Zero;
            *(cardinalityArray[0, 4]) = None;
            *(cardinalityArray[0, 5]) = Zero;
            *(cardinalityArray[0, 6]) = None;
            *(cardinalityArray[0, 7]) = Zero;
            *(cardinalityArray[1, 0]) = Zero;
            *(cardinalityArray[1, 1]) = Zero;
            *(cardinalityArray[1, 2]) = Zero;
            *(cardinalityArray[1, 3]) = Zero;
            *(cardinalityArray[1, 4]) = Zero;
            *(cardinalityArray[1, 5]) = Zero;
            *(cardinalityArray[1, 6]) = Zero;
            *(cardinalityArray[1, 7]) = Zero;
            *(cardinalityArray[2, 0]) = None;
            *(cardinalityArray[2, 1]) = Zero;
            *(cardinalityArray[2, 2]) = One;
            *(cardinalityArray[2, 3]) = ZeroOrOne;
            *(cardinalityArray[2, 4]) = More;
            *(cardinalityArray[2, 5]) = NotOne;
            *(cardinalityArray[2, 6]) = OneOrMore;
            *(cardinalityArray[2, 7]) = ZeroOrMore;
            *(cardinalityArray[3, 0]) = Zero;
            *(cardinalityArray[3, 1]) = Zero;
            *(cardinalityArray[3, 2]) = ZeroOrOne;
            *(cardinalityArray[3, 3]) = ZeroOrOne;
            *(cardinalityArray[3, 4]) = NotOne;
            *(cardinalityArray[3, 5]) = NotOne;
            *(cardinalityArray[3, 6]) = ZeroOrMore;
            *(cardinalityArray[3, 7]) = ZeroOrMore;
            *(cardinalityArray[4, 0]) = None;
            *(cardinalityArray[4, 1]) = Zero;
            *(cardinalityArray[4, 2]) = More;
            *(cardinalityArray[4, 3]) = NotOne;
            *(cardinalityArray[4, 4]) = More;
            *(cardinalityArray[4, 5]) = NotOne;
            *(cardinalityArray[4, 6]) = More;
            *(cardinalityArray[4, 7]) = NotOne;
            *(cardinalityArray[5, 0]) = Zero;
            *(cardinalityArray[5, 1]) = Zero;
            *(cardinalityArray[5, 2]) = NotOne;
            *(cardinalityArray[5, 3]) = NotOne;
            *(cardinalityArray[5, 4]) = NotOne;
            *(cardinalityArray[5, 5]) = NotOne;
            *(cardinalityArray[5, 6]) = NotOne;
            *(cardinalityArray[5, 7]) = NotOne;
            *(cardinalityArray[6, 0]) = None;
            *(cardinalityArray[6, 1]) = Zero;
            *(cardinalityArray[6, 2]) = OneOrMore;
            *(cardinalityArray[6, 3]) = ZeroOrMore;
            *(cardinalityArray[6, 4]) = More;
            *(cardinalityArray[6, 5]) = NotOne;
            *(cardinalityArray[6, 6]) = OneOrMore;
            *(cardinalityArray[6, 7]) = ZeroOrMore;
            *(cardinalityArray[7, 0]) = Zero;
            *(cardinalityArray[7, 1]) = Zero;
            *(cardinalityArray[7, 2]) = ZeroOrMore;
            *(cardinalityArray[7, 3]) = ZeroOrMore;
            *(cardinalityArray[7, 4]) = NotOne;
            *(cardinalityArray[7, 5]) = NotOne;
            *(cardinalityArray[7, 6]) = ZeroOrMore;
            *(cardinalityArray[7, 7]) = ZeroOrMore;
            cardinalityProduct = cardinalityArray;
            XmlQueryCardinality[,] cardinalityArray2 = new XmlQueryCardinality[8, 8];
            *(cardinalityArray2[0, 0]) = None;
            *(cardinalityArray2[0, 1]) = Zero;
            *(cardinalityArray2[0, 2]) = One;
            *(cardinalityArray2[0, 3]) = ZeroOrOne;
            *(cardinalityArray2[0, 4]) = More;
            *(cardinalityArray2[0, 5]) = NotOne;
            *(cardinalityArray2[0, 6]) = OneOrMore;
            *(cardinalityArray2[0, 7]) = ZeroOrMore;
            *(cardinalityArray2[1, 0]) = Zero;
            *(cardinalityArray2[1, 1]) = Zero;
            *(cardinalityArray2[1, 2]) = One;
            *(cardinalityArray2[1, 3]) = ZeroOrOne;
            *(cardinalityArray2[1, 4]) = More;
            *(cardinalityArray2[1, 5]) = NotOne;
            *(cardinalityArray2[1, 6]) = OneOrMore;
            *(cardinalityArray2[1, 7]) = ZeroOrMore;
            *(cardinalityArray2[2, 0]) = One;
            *(cardinalityArray2[2, 1]) = One;
            *(cardinalityArray2[2, 2]) = More;
            *(cardinalityArray2[2, 3]) = OneOrMore;
            *(cardinalityArray2[2, 4]) = More;
            *(cardinalityArray2[2, 5]) = OneOrMore;
            *(cardinalityArray2[2, 6]) = More;
            *(cardinalityArray2[2, 7]) = OneOrMore;
            *(cardinalityArray2[3, 0]) = ZeroOrOne;
            *(cardinalityArray2[3, 1]) = ZeroOrOne;
            *(cardinalityArray2[3, 2]) = OneOrMore;
            *(cardinalityArray2[3, 3]) = ZeroOrMore;
            *(cardinalityArray2[3, 4]) = More;
            *(cardinalityArray2[3, 5]) = ZeroOrMore;
            *(cardinalityArray2[3, 6]) = OneOrMore;
            *(cardinalityArray2[3, 7]) = ZeroOrMore;
            *(cardinalityArray2[4, 0]) = More;
            *(cardinalityArray2[4, 1]) = More;
            *(cardinalityArray2[4, 2]) = More;
            *(cardinalityArray2[4, 3]) = More;
            *(cardinalityArray2[4, 4]) = More;
            *(cardinalityArray2[4, 5]) = More;
            *(cardinalityArray2[4, 6]) = More;
            *(cardinalityArray2[4, 7]) = More;
            *(cardinalityArray2[5, 0]) = NotOne;
            *(cardinalityArray2[5, 1]) = NotOne;
            *(cardinalityArray2[5, 2]) = OneOrMore;
            *(cardinalityArray2[5, 3]) = ZeroOrMore;
            *(cardinalityArray2[5, 4]) = More;
            *(cardinalityArray2[5, 5]) = NotOne;
            *(cardinalityArray2[5, 6]) = OneOrMore;
            *(cardinalityArray2[5, 7]) = ZeroOrMore;
            *(cardinalityArray2[6, 0]) = OneOrMore;
            *(cardinalityArray2[6, 1]) = OneOrMore;
            *(cardinalityArray2[6, 2]) = More;
            *(cardinalityArray2[6, 3]) = OneOrMore;
            *(cardinalityArray2[6, 4]) = More;
            *(cardinalityArray2[6, 5]) = OneOrMore;
            *(cardinalityArray2[6, 6]) = More;
            *(cardinalityArray2[6, 7]) = OneOrMore;
            *(cardinalityArray2[7, 0]) = ZeroOrMore;
            *(cardinalityArray2[7, 1]) = ZeroOrMore;
            *(cardinalityArray2[7, 2]) = OneOrMore;
            *(cardinalityArray2[7, 3]) = ZeroOrMore;
            *(cardinalityArray2[7, 4]) = More;
            *(cardinalityArray2[7, 5]) = ZeroOrMore;
            *(cardinalityArray2[7, 6]) = OneOrMore;
            *(cardinalityArray2[7, 7]) = ZeroOrMore;
            cardinalitySum = cardinalityArray2;
            toString = new string[] { "", "?", "", "?", "+", "*", "+", "*" };
            serialized = new string[] { "None", "Zero", "One", "ZeroOrOne", "More", "NotOne", "OneOrMore", "ZeroOrMore" };
        }
    }
}

