namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XPathFollowingIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private bool needFirst;
        public void Create(XPathNavigator input, XmlNavigatorFilter filter)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
            this.filter = filter;
            this.needFirst = true;
        }

        public bool MoveNext()
        {
            if (!this.needFirst)
            {
                return this.filter.MoveToFollowing(this.navCurrent, null);
            }
            if (!MoveFirst(this.filter, this.navCurrent))
            {
                return false;
            }
            this.needFirst = false;
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
        internal static bool MoveFirst(XmlNavigatorFilter filter, XPathNavigator nav)
        {
            if ((nav.NodeType == XPathNodeType.Attribute) || (nav.NodeType == XPathNodeType.Namespace))
            {
                if (!nav.MoveToParent())
                {
                    return false;
                }
                if (!filter.MoveToFollowing(nav, null))
                {
                    return false;
                }
            }
            else
            {
                if (!nav.MoveToNonDescendant())
                {
                    return false;
                }
                if (filter.IsFiltered(nav) && !filter.MoveToFollowing(nav, null))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

