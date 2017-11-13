namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct AncestorIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private bool haveCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter, bool orSelf)
        {
            this.filter = filter;
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.haveCurrent = orSelf && !this.filter.IsFiltered(this.navCurrent);
        }

        public bool MoveNext()
        {
            if (!this.haveCurrent)
            {
                while (this.navCurrent.MoveToParent())
                {
                    if (!this.filter.IsFiltered(this.navCurrent))
                    {
                        return true;
                    }
                }
                return false;
            }
            this.haveCurrent = false;
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

