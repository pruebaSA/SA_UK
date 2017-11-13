namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XPathPrecedingIterator
    {
        private XmlNavigatorStack stack;
        private XPathNavigator navCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter)
        {
            XPathPrecedingDocOrderIterator iterator = new XPathPrecedingDocOrderIterator();
            iterator.Create(context, filter);
            this.stack.Reset();
            while (iterator.MoveNext())
            {
                this.stack.Push(iterator.Current.Clone());
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

