namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct PrecedingSiblingIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.filter = filter;
        }

        public bool MoveNext() => 
            this.filter.MoveToPreviousSibling(this.navCurrent);

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

