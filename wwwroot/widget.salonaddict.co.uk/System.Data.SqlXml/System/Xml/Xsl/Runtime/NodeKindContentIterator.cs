namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct NodeKindContentIterator
    {
        private XPathNodeType nodeType;
        private XPathNavigator navCurrent;
        private bool needFirst;
        public void Create(XPathNavigator context, XPathNodeType nodeType)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.nodeType = nodeType;
            this.needFirst = true;
        }

        public bool MoveNext()
        {
            if (this.needFirst)
            {
                this.needFirst = !this.navCurrent.MoveToChild(this.nodeType);
                return !this.needFirst;
            }
            return this.navCurrent.MoveToNext(this.nodeType);
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

