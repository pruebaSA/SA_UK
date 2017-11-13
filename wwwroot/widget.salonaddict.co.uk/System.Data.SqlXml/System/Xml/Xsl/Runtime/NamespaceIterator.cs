namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct NamespaceIterator
    {
        private XPathNavigator navCurrent;
        private XmlNavigatorStack navStack;
        public void Create(XPathNavigator context)
        {
            this.navStack.Reset();
            if (context.MoveToFirstNamespace(XPathNamespaceScope.All))
            {
                do
                {
                    if ((context.LocalName.Length != 0) || (context.Value.Length != 0))
                    {
                        this.navStack.Push(context.Clone());
                    }
                }
                while (context.MoveToNextNamespace(XPathNamespaceScope.All));
                context.MoveToParent();
            }
        }

        public bool MoveNext()
        {
            if (this.navStack.IsEmpty)
            {
                return false;
            }
            this.navCurrent = this.navStack.Pop();
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

