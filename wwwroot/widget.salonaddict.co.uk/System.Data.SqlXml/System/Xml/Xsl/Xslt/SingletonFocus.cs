namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SingletonFocus : IFocus
    {
        private XPathQilFactory f;
        private SingletonFocusType focusType;
        private QilIterator current;
        public SingletonFocus(XPathQilFactory f)
        {
            this.f = f;
            this.focusType = SingletonFocusType.None;
            this.current = null;
        }

        public void SetFocus(SingletonFocusType focusType)
        {
            this.focusType = focusType;
        }

        public void SetFocus(QilIterator current)
        {
            if (current != null)
            {
                this.focusType = SingletonFocusType.Iterator;
                this.current = current;
            }
            else
            {
                this.focusType = SingletonFocusType.None;
                this.current = null;
            }
        }

        [Conditional("DEBUG")]
        private void CheckFocus()
        {
        }

        public QilNode GetCurrent()
        {
            switch (this.focusType)
            {
                case SingletonFocusType.InitialDocumentNode:
                    return this.f.Root(this.f.XmlContext());

                case SingletonFocusType.InitialContextNode:
                    return this.f.XmlContext();
            }
            return this.current;
        }

        public QilNode GetPosition() => 
            this.f.Double(1.0);

        public QilNode GetLast() => 
            this.f.Double(1.0);
    }
}

