namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Xml.XPath;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlQueryItemSequence : XmlQuerySequence<XPathItem>
    {
        public static readonly XmlQueryItemSequence Empty = new XmlQueryItemSequence();

        public XmlQueryItemSequence()
        {
        }

        public XmlQueryItemSequence(int capacity) : base(capacity)
        {
        }

        public XmlQueryItemSequence(XPathItem item) : base(1)
        {
            this.AddClone(item);
        }

        public void AddClone(XPathItem item)
        {
            if (item.IsNode)
            {
                base.Add(((XPathNavigator) item).Clone());
            }
            else
            {
                base.Add(item);
            }
        }

        public static XmlQueryItemSequence CreateOrReuse(XmlQueryItemSequence seq)
        {
            if (seq != null)
            {
                seq.Clear();
                return seq;
            }
            return new XmlQueryItemSequence();
        }

        public static XmlQueryItemSequence CreateOrReuse(XmlQueryItemSequence seq, XPathItem item)
        {
            if (seq != null)
            {
                seq.Clear();
                seq.Add(item);
                return seq;
            }
            return new XmlQueryItemSequence(item);
        }
    }
}

