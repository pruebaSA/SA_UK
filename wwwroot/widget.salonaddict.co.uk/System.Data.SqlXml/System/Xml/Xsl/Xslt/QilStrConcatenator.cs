namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    internal class QilStrConcatenator
    {
        private BufferBuilder builder;
        private QilList concat;
        private XPathQilFactory f;
        private bool inUse;

        public QilStrConcatenator(XPathQilFactory f)
        {
            this.f = f;
            this.builder = new BufferBuilder();
        }

        public void Append(char value)
        {
            this.builder.Append(value);
        }

        public void Append(string value)
        {
            this.builder.Append(value);
        }

        public void Append(QilNode value)
        {
            if (value != null)
            {
                if (value.NodeType == QilNodeType.LiteralString)
                {
                    this.builder.Append((string) ((QilLiteral) value));
                }
                else
                {
                    this.FlushBuilder();
                    this.concat.Add(value);
                }
            }
        }

        private void FlushBuilder()
        {
            if (this.concat == null)
            {
                this.concat = this.f.BaseFactory.Sequence();
            }
            if (this.builder.Length != 0)
            {
                this.concat.Add((QilNode) this.f.String(this.builder.ToString()));
                this.builder.Length = 0;
            }
        }

        public void Reset()
        {
            this.inUse = true;
            this.builder.Clear();
            this.concat = null;
        }

        public QilNode ToQil()
        {
            this.inUse = false;
            if (this.concat == null)
            {
                return this.f.String(this.builder.ToString());
            }
            this.FlushBuilder();
            return this.f.StrConcat((QilNode) this.concat);
        }
    }
}

