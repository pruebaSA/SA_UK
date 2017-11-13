namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.IO;

    internal class XmlQueryDataReader : BinaryReader
    {
        public XmlQueryDataReader(Stream input) : base(input)
        {
        }

        public int ReadInt32Encoded() => 
            base.Read7BitEncodedInt();

        public sbyte ReadSByte(sbyte minValue, sbyte maxValue)
        {
            sbyte num = this.ReadSByte();
            if ((num < minValue) || (maxValue < num))
            {
                throw new ArgumentOutOfRangeException();
            }
            return num;
        }

        public string ReadStringQ()
        {
            if (!this.ReadBoolean())
            {
                return null;
            }
            return this.ReadString();
        }
    }
}

