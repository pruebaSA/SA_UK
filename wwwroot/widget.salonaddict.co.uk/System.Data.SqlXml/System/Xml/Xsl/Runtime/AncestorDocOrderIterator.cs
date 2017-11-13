namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct AncestorDocOrderIterator
    {
        private XmlNavigatorStack stack;
        private XPathNavigator navCurrent;
        public void Create(XPathNavigator context, XmlNavigatorFilter filter, bool orSelf)
        {
            AncestorIterator iterator = new AncestorIterator();
            iterator.Create(context, filter, orSelf);
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

