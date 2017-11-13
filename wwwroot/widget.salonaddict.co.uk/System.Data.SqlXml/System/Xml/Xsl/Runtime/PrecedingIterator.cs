namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct PrecedingIterator
    {
        private XmlNavigatorStack stack;
        private XPathNavigator navCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.navCurrent.MoveToRoot();
            this.stack.Reset();
            if (!this.navCurrent.IsSamePosition(context))
            {
                if (!filter.IsFiltered(this.navCurrent))
                {
                    this.stack.Push(this.navCurrent.Clone());
                }
                while (filter.MoveToFollowing(this.navCurrent, context))
                {
                    this.stack.Push(this.navCurrent.Clone());
                }
            }
        }

        public bool MoveNext()
        {
            if (this.stack.IsEmpty)
            {
                return false;
            }
            this.navCurrent = this.stack.Pop();
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

