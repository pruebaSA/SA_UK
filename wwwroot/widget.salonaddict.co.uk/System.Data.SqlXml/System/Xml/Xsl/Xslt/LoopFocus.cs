namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    [StructLayout(LayoutKind.Sequential)]
    internal struct LoopFocus : IFocus
    {
        private XPathQilFactory f;
        private QilIterator current;
        private QilIterator cached;
        private QilIterator last;
        public LoopFocus(XPathQilFactory f)
        {
            this.f = f;
            this.current = this.cached = (QilIterator) (this.last = null);
        }

        public void SetFocus(QilIterator current)
        {
            this.current = current;
            this.cached = (QilIterator) (this.last = null);
        }

        public bool IsFocusSet =>
            (this.current != null);
        public QilNode GetCurrent() => 
            this.current;

        public QilNode GetPosition() => 
            this.f.XsltConvert(this.f.PositionOf(this.current), XmlQueryTypeFactory.DoubleX);

        public QilNode GetLast()
        {
            if (this.last == null)
            {
                this.last = this.f.Let(this.f.Double(0.0));
            }
            return this.last;
        }

        public void EnsureCache()
        {
            if (this.cached == null)
            {
                this.cached = this.f.Let(this.current.Binding);
                this.current.Binding = this.cached;
            }
        }

        public void Sort(QilNode sortKeys)
        {
            if (sortKeys != null)
            {
                this.EnsureCache();
                this.current = this.f.For(this.f.Sort(this.current, sortKeys));
            }
        }

        public QilLoop ConstructLoop(QilNode body)
        {
            if (this.last != null)
            {
                this.EnsureCache();
                this.last.Binding = this.f.XsltConvert(this.f.Length(this.cached), XmlQueryTypeFactory.DoubleX);
            }
            QilLoop loop = this.f.BaseFactory.Loop(this.current, body);
            if (this.last != null)
            {
                loop = this.f.BaseFactory.Loop(this.last, loop);
            }
            if (this.cached != null)
            {
                loop = this.f.BaseFactory.Loop(this.cached, loop);
            }
            return loop;
        }
    }
}

