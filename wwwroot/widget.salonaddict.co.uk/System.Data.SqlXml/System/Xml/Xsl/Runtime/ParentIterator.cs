namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct ParentIterator
    {
        private XPathNavigator navCurrent;
        private bool haveCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.haveCurrent = this.navCurrent.MoveToParent() && !filter.IsFiltered(this.navCurrent);
        }

        public bool MoveNext()
        {
            if (this.haveCurrent)
            {
                this.haveCurrent = false;
                return true;
            }
            return false;
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

