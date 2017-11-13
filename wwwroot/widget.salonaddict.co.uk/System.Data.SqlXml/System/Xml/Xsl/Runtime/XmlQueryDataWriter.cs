namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.IO;

    internal class XmlQueryDataWriter : BinaryWriter
    {
        public XmlQueryDataWriter(Stream output) : base(output)
        {
        }

        public void WriteInt32Encoded(int value)
        {
            base.Write7BitEncodedInt(value);
        }

        public void WriteStringQ(string value)
        {
            this.Write(value != null);
            if (value != null)
            {
                this.Write(value);
            }
        }
    }
}

