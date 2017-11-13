namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct NodeRangeIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private XPathNavigator navEnd;
        private IteratorState state;
        public void Create(XPathNavigator start, XmlNavigatorFilter filter, XPathNavigator end)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, start);
            this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, end);
            this.filter = filter;
            if (start.IsSamePosition(end))
            {
                this.state = !filter.IsFiltered(start) ? IteratorState.HaveCurrentNoNext : IteratorState.NoNext;
            }
            else
            {
                this.state = !filter.IsFiltered(start) ? IteratorState.HaveCurrent : IteratorState.NeedCurrent;
            }
        }

        public bool MoveNext()
        {
            switch (this.state)
            {
                case IteratorState.HaveCurrent:
                    this.state = IteratorState.NeedCurrent;
                    return true;

                case IteratorState.NeedCurrent:
                    if (this.filter.MoveToFollowing(this.navCurrent, this.navEnd))
                    {
                        break;
                    }
                    if (!this.filter.IsFiltered(this.navEnd))
                    {
                        this.navCurrent.MoveTo(this.navEnd);
                        this.state = IteratorState.NoNext;
                        break;
                    }
                    this.state = IteratorState.NoNext;
                    return false;

                case IteratorState.HaveCurrentNoNext:
                    this.state = IteratorState.NoNext;
                    return true;

                default:
                    return false;
            }
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
        private enum IteratorState
        {
            HaveCurrent,
            NeedCurrent,
            HaveCurrentNoNext,
            NoNext
        }
    }
}

