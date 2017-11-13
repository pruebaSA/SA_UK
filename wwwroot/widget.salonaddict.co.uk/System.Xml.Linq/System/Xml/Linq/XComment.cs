namespace System.Xml.Linq
{
    using System;
    using System.Xml;

    public class XComment : XNode
    {
        internal string value;

        public XComment(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.value = value;
        }

        public XComment(XComment other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            this.value = other.value;
        }

        internal XComment(XmlReader r)
        {
            this.value = r.Value;
            r.Read();
        }

        internal override XNode CloneNode() => 
            new XComment(this);

        internal override bool DeepEquals(XNode node)
        {
            XComment comment = node as XComment;
            return ((comment != null) && (this.value == comment.value));
        }

        internal override int GetDeepHashCode() => 
            this.value.GetHashCode();

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteComment(this.value);
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.Comment;

        public string Value
        {
            get => 
                this.value;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.value = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }
    }
}

