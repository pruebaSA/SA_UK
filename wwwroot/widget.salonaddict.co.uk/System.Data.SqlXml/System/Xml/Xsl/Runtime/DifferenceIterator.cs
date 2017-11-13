namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct DifferenceIterator
    {
        private XmlQueryRuntime runtime;
        private XPathNavigator navLeft;
        private XPathNavigator navRight;
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
                    this.navLeft = nestedNavigator;
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.InitRightIterator;

                case IteratorState.NeedLeft:
                    this.navLeft = nestedNavigator;
                    break;

                case IteratorState.NeedRight:
                    this.navRight = nestedNavigator;
                    break;

                case IteratorState.NeedLeftAndRight:
                    this.navLeft = nestedNavigator;
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.NeedRightNode;

                case IteratorState.HaveCurrent:
                    this.state = IteratorState.NeedLeft;
                    return SetIteratorResult.NeedLeftNode;
            }
            if (this.navLeft == null)
            {
                return SetIteratorResult.NoMoreNodes;
            }
            if (this.navRight != null)
            {
                int num = this.runtime.ComparePosition(this.navLeft, this.navRight);
                if (num == 0)
                {
                    this.state = IteratorState.NeedLeftAndRight;
                    return SetIteratorResult.NeedLeftNode;
                }
                if (num > 0)
                {
                    this.state = IteratorState.NeedRight;
                    return SetIteratorResult.NeedRightNode;
                }
            }
            this.state = IteratorState.HaveCurrent;
            return SetIteratorResult.HaveCurrentNode;
        }

        public XPathNavigator Current =>
            this.navLeft;
        private enum IteratorState
        {
            InitLeft,
            NeedLeft,
            NeedRight,
            NeedLeftAndRight,
            HaveCurrent
        }
    }
}

