namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XPathPrecedingMergeIterator
    {
        private XmlNavigatorFilter filter;
        private IteratorState state;
        private XPathNavigator navCurrent;
        private XPathNavigator navNext;
        private XmlNavigatorStack navStack;
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
                    if (input != null)
                    {
                        this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
                        this.state = IteratorState.HaveCandidateCurrent;
                        return IteratorResult.NeedInputNode;
                    }
                    return IteratorResult.NoMoreNodes;

                case IteratorState.HaveCandidateCurrent:
                    if (input != null)
                    {
                        if (this.navCurrent.ComparePosition(input) != XmlNodeOrder.Unknown)
                        {
                            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
                            return IteratorResult.NeedInputNode;
                        }
                        this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
                        this.state = IteratorState.HaveCurrentHaveNext;
                        break;
                    }
                    this.state = IteratorState.HaveCurrentNoNext;
                    break;

                default:
                    goto Label_0085;
            }
            this.PushAncestors();
        Label_0085:
            if (!this.navStack.IsEmpty)
            {
                do
                {
                    if (this.filter.MoveToFollowing(this.navCurrent, this.navStack.Peek()))
                    {
                        return IteratorResult.HaveCurrentNode;
                    }
                    this.navCurrent.MoveTo(this.navStack.Pop());
                }
                while (!this.navStack.IsEmpty);
            }
            if (this.state == IteratorState.HaveCurrentNoNext)
            {
                this.state = IteratorState.NeedCandidateCurrent;
                return IteratorResult.NoMoreNodes;
            }
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, this.navNext);
            this.state = IteratorState.HaveCandidateCurrent;
            return IteratorResult.HaveCurrentNode;
        }

        public XPathNavigator Current =>
            this.navCurrent;
        private void PushAncestors()
        {
            this.navStack.Reset();
            do
            {
                this.navStack.Push(this.navCurrent.Clone());
            }
            while (this.navCurrent.MoveToParent());
            this.navStack.Pop();
        }
        private enum IteratorState
        {
            NeedCandidateCurrent,
            HaveCandidateCurrent,
            HaveCurrentHaveNext,
            HaveCurrentNoNext
        }
    }
}

