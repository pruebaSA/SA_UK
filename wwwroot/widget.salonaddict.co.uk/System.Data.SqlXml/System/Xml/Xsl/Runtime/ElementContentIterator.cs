namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct ElementContentIterator
    {
        private string localName;
        private string ns;
        private XPathNavigator navCurrent;
        private bool needFirst;
        public void Create(XPathNavigator context, string localName, string ns)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.localName = localName;
            this.ns = ns;
            this.needFirst = true;
        }

        public bool MoveNext()
        {
            if (this.needFirst)
            {
                this.needFirst = !this.navCurrent.MoveToChild(this.localName, this.ns);
                return !this.needFirst;
            }
            return this.navCurrent.MoveToNext(this.localName, this.ns);
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

