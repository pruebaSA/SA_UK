namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XPathFollowingMergeIterator
    {
        private XmlNavigatorFilter filter;
        private IteratorState state;
        private XPathNavigator navCurrent;
        private XPathNavigator navNext;
        public void Create(XmlNavigatorFilter filter)
        {
            this.filter = filter;
            this.state = IteratorState.NeedCandidateCurrent;
        }

        public IteratorResult MoveNext(XPathNavigator input)
        {
            switch (this.state)
            {
                case IteratorState.NeedCandidateCurrent:
                    break;

                case IteratorState.HaveCandidateCurrent:
                    if (input != null)
                    {
                        if (this.navCurrent.IsDescendant(input))
                        {
                            break;
                        }
                        this.state = IteratorState.HaveCurrentNeedNext;
                        goto Label_0064;
                    }
                    this.state = IteratorState.HaveCurrentNoNext;
                    return this.MoveFirst();

                case IteratorState.HaveCurrentNeedNext:
                    goto Label_0064;

                default:
                    if (!this.filter.MoveToFollowing(this.navCurrent, null))
                    {
                        return this.MoveFailed();
                    }
                    return IteratorResult.HaveCurrentNode;
            }
            if (input == null)
            {
                return IteratorResult.NoMoreNodes;
            }
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
            this.state = IteratorState.HaveCandidateCurrent;
            return IteratorResult.NeedInputNode;
        Label_0064:
            if (input == null)
            {
                this.state = IteratorState.HaveCurrentNoNext;
                return this.MoveFirst();
            }
            if (this.navCurrent.ComparePosition(input) != XmlNodeOrder.Unknown)
            {
                return IteratorResult.NeedInputNode;
            }
            this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
            this.state = IteratorState.HaveCurrentHaveNext;
            return this.MoveFirst();
        }

        public XPathNavigator Current =>
            this.navCurrent;
        private IteratorResult MoveFailed()
        {
            if (this.state == IteratorState.HaveCurrentNoNext)
            {
                this.state = IteratorState.NeedCandidateCurrent;
                return IteratorResult.NoMoreNodes;
            }
            this.state = IteratorState.HaveCandidateCurrent;
            XPathNavigator navCurrent = this.navCurrent;
            this.navCurrent = this.navNext;
            this.navNext = navCurrent;
            return IteratorResult.NeedInputNode;
        }

        private IteratorResult MoveFirst()
        {
            if (!XPathFollowingIterator.MoveFirst(this.filter, this.navCurrent))
            {
                return this.MoveFailed();
            }
            return IteratorResult.HaveCurrentNode;
        }
        private enum IteratorState
        {
            NeedCandidateCurrent,
            HaveCandidateCurrent,
            HaveCurrentNeedNext,
            HaveCurrentHaveNext,
            HaveCurrentNoNext
        }
    }
}

