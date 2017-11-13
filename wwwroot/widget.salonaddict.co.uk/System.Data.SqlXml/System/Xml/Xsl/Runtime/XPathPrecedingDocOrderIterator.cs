namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XPathPrecedingDocOrderIterator
    {
        private XmlNavigatorFilter filter;
        private XPathNavigator navCurrent;
        private XmlNavigatorStack navStack;
        public void Create(XPathNavigator input, XmlNavigatorFilter filter)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
            this.filter = filter;
            this.PushAncestors();
        }

        public bool MoveNext()
        {
            if (!this.navStack.IsEmpty)
            {
                do
                {
                    if (this.filter.MoveToFollowing(this.navCurrent, this.navStack.Peek()))
                    {
                        return true;
                    }
                    this.navCurrent.MoveTo(this.navStack.Pop());
                }
                while (!this.navStack.IsEmpty);
            }
            return false;
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
    }
}

