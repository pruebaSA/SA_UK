namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct DescendantIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private XPathNavigator navEnd;
        private bool hasFirst;
        public void Create(XPathNavigator input, XmlNavigatorFilter filter, bool orSelf)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
            this.filter = filter;
            if (input.NodeType == XPathNodeType.Root)
            {
                this.navEnd = null;
            }
            else
            {
                this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, input);
                this.navEnd.MoveToNonDescendant();
            }
            this.hasFirst = orSelf && !this.filter.IsFiltered(this.navCurrent);
        }

        public bool MoveNext()
        {
            if (this.hasFirst)
            {
                this.hasFirst = false;
                return true;
            }
            return this.filter.MoveToFollowing(this.navCurrent, this.navEnd);
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

