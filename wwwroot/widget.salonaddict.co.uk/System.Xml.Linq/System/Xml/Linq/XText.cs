﻿namespace System.Xml.Linq
{
    using System;
    using System.Text;
    using System.Xml;

    public class XText : XNode
    {
        internal string text;

        public XText(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.text = value;
        }

        public XText(XText other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            this.text = other.text;
        }

        internal XText(XmlReader r)
        {
            this.text = r.Value;
            r.Read();
        }

        internal override void AppendText(StringBuilder sb)
        {
            sb.Append(this.text);
        }

        internal override XNode CloneNode() => 
            new XText(this);

        internal override bool DeepEquals(XNode node) => 
            (((node != null) && (this.NodeType == node.NodeType)) && (this.text == ((XText) node).text));

        internal override int GetDeepHashCode() => 
            this.text.GetHashCode();

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (base.parent is XDocument)
            {
                writer.WriteWhitespace(this.text);
            }
            else
            {
                writer.WriteString(this.text);
            }
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.Text;

        public string Value
        {
            get => 
                this.text;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.text = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }
    }
}

