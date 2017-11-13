namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct UnionIterator
    {
        private XmlQueryRuntime runtime;
        private XPathNavigator navCurr;
        private XPathNavigator navOther;
        private IteratorState state;
        public void Create(XmlQueryRuntime runtime)
        {
            this.runtime = runtime;
            this.state = IteratorState.InitLeft;
        }

        public SetIteratorResult MoveNext(XPathNavigator nestedNavigator)
        {
            switch (this.state)
            {
                case IteratorState.InitLeft:
                    this.navOther = nestedNavigator;
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.InitRightIterator;

                case IteratorState.NeedLeft:
                    this.navCurr = nestedNavigator;
                    this.state = IteratorState.LeftIsCurrent;
                    break;

                case IteratorState.NeedRight:
                    this.navCurr = nestedNavigator;
                    this.state = IteratorState.RightIsCurrent;
                    break;

                case IteratorState.LeftIsCurrent:
                    this.state = IteratorState.NeedLeft;
                    return SetIteratorResult.NeedLeftNode;

                case IteratorState.RightIsCurrent:
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.NeedRightNode;
            }
            if (this.navCurr == null)
            {
                if (this.navOther == null)
                {
                    return SetIteratorResult.NoMoreNodes;
                }
                this.Swap();
            }
            else if (this.navOther != null)
            {
                int num = this.runtime.ComparePosition(this.navOther, this.navCurr);
                if (num == 0)
                {
                    if (this.state == IteratorState.LeftIsCurrent)
                    {
                        this.state = IteratorState.NeedLeft;
                        return SetIteratorResult.NeedLeftNode;
                    }
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.NeedRightNode;
                }
                if (num < 0)
                {
                    this.Swap();
                }
            }
            return SetIteratorResult.HaveCurrentNode;
        }

        public XPathNavigator Current =>
            this.navCurr;
        private void Swap()
        {
            XPathNavigator navCurr = this.navCurr;
            this.navCurr = this.navOther;
            this.navOther = navCurr;
            if (this.state == IteratorState.LeftIsCurrent)
            {
                this.state = IteratorState.RightIsCurrent;
            }
            else
            {
                this.state = IteratorState.LeftIsCurrent;
            }
        }
        private enum IteratorState
        {
            InitLeft,
            NeedLeft,
            NeedRight,
            LeftIsCurrent,
            RightIsCurrent
        }
    }
}

