namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct DescendantMergeIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private XPathNavigator navRoot;
        private XPathNavigator navEnd;
        private IteratorState state;
        private bool orSelf;
        public void Create(XmlNavigatorFilter filter, bool orSelf)
        {
            this.filter = filter;
            this.state = IteratorState.NoPrevious;
            this.orSelf = orSelf;
        }

        public IteratorResult MoveNext(XPathNavigator input)
        {
            if (this.state != IteratorState.NeedDescendant)
            {
                if (input == null)
                {
                    return IteratorResult.NoMoreNodes;
                }
                if ((this.state != IteratorState.NoPrevious) && this.navRoot.IsDescendant(input))
                {
                    return IteratorResult.NeedInputNode;
                }
                this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
                this.navRoot = XmlQueryRuntime.SyncToNavigator(this.navRoot, input);
                this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, input);
                this.navEnd.MoveToNonDescendant();
                this.state = IteratorState.NeedDescendant;
                if (this.orSelf && !this.filter.IsFiltered(input))
                {
                    return IteratorResult.HaveCurrentNode;
                }
            }
            if (this.filter.MoveToFollowing(this.navCurrent, this.navEnd))
            {
                return IteratorResult.HaveCurrentNode;
            }
            this.state = IteratorState.NeedCurrent;
            return IteratorResult.NeedInputNode;
        }

        public XPathNavigator Current =>
            this.navCurrent;
        private enum IteratorState
        {
            NoPrevious,
            NeedCurrent,
            NeedDescendant
        }
    }
}

