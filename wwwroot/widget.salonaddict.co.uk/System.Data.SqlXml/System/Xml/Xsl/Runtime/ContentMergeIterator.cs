namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct ContentMergeIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private XPathNavigator navNext;
        private XmlNavigatorStack navStack;
        private IteratorState state;
        public void Create(XmlNavigatorFilter filter)
        {
            this.filter = filter;
            this.navStack.Reset();
            this.state = IteratorState.NeedCurrent;
        }

        public IteratorResult MoveNext(XPathNavigator input) => 
            this.MoveNext(input, true);

        internal IteratorResult MoveNext(XPathNavigator input, bool isContent)
        {
            switch (this.state)
            {
                case IteratorState.NeedCurrent:
                    if (input != null)
                    {
                        this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
                        if (isContent ? this.filter.MoveToContent(this.navCurrent) : this.filter.MoveToFollowingSibling(this.navCurrent))
                        {
                            this.state = IteratorState.HaveCurrentNeedNext;
                        }
                        return IteratorResult.NeedInputNode;
                    }
                    return IteratorResult.NoMoreNodes;

                case IteratorState.HaveCurrentNeedNext:
                    if (input != null)
                    {
                        this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
                        if (isContent ? this.filter.MoveToContent(this.navNext) : this.filter.MoveToFollowingSibling(this.navNext))
                        {
                            this.state = IteratorState.HaveCurrentHaveNext;
                            return this.DocOrderMerge();
                        }
                        return IteratorResult.NeedInputNode;
                    }
                    this.state = IteratorState.HaveCurrentNoNext;
                    return IteratorResult.HaveCurrentNode;

                case IteratorState.HaveCurrentNoNext:
                case IteratorState.HaveCurrentHaveNext:
                    if (!(isContent ? !this.filter.MoveToNextContent(this.navCurrent) : !this.filter.MoveToFollowingSibling(this.navCurrent)))
                    {
                        break;
                    }
                    if (!this.navStack.IsEmpty)
                    {
                        this.navCurrent = this.navStack.Pop();
                        break;
                    }
                    if (this.state != IteratorState.HaveCurrentNoNext)
                    {
                        this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, this.navNext);
                        this.state = IteratorState.HaveCurrentNeedNext;
                        return IteratorResult.NeedInputNode;
                    }
                    return IteratorResult.NoMoreNodes;

                default:
                    return IteratorResult.NoMoreNodes;
            }
            if (this.state == IteratorState.HaveCurrentNoNext)
            {
                return IteratorResult.HaveCurrentNode;
            }
            return this.DocOrderMerge();
        }

        public XPathNavigator Current =>
            this.navCurrent;
        private IteratorResult DocOrderMerge()
        {
            XmlNodeOrder order = this.navCurrent.ComparePosition(this.navNext);
            switch (order)
            {
                case XmlNodeOrder.Before:
                case XmlNodeOrder.Unknown:
                    return IteratorResult.HaveCurrentNode;
            }
            if (order == XmlNodeOrder.After)
            {
                this.navStack.Push(this.navCurrent);
                this.navCurrent = this.navNext;
                this.navNext = null;
            }
            this.state = IteratorState.HaveCurrentNeedNext;
            return IteratorResult.NeedInputNode;
        }
        private enum IteratorState
        {
            NeedCurrent,
            HaveCurrentNeedNext,
            HaveCurrentNoNext,
            HaveCurrentHaveNext
        }
    }
}

