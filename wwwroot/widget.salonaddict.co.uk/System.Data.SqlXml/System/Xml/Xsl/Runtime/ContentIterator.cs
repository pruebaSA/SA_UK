namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct ContentIterator
    {
        private XPathNavigator navCurrent;
        private bool needFirst;
        public void Create(XPathNavigator context)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.needFirst = true;
        }

        public bool MoveNext()
        {
            if (this.needFirst)
            {
                this.needFirst = !this.navCurrent.MoveToFirstChild();
                return !this.needFirst;
            }
            return this.navCurrent.MoveToNext();
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

