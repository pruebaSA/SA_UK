namespace System.Xml.Linq
{
    using System;
    using System.Xml;

    public class XCData : XText
    {
        public XCData(string value) : base(value)
        {
        }

        public XCData(XCData other) : base(other)
        {
        }

        internal XCData(XmlReader r) : base(r)
        {
        }

        internal override XNode CloneNode() => 
            new XCData(this);

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteCData(base.text);
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.CDATA;
    }
}

