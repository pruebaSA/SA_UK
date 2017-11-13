namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct FollowingSiblingMergeIterator
    {
        private ContentMergeIterator wrapped;
        public void Create(XmlNavigatorFilter filter)
        {
            this.wrapped.Create(filter);
        }

        public IteratorResult MoveNext(XPathNavigator navigator) => 
            this.wrapped.MoveNext(navigator, false);

        public XPathNavigator Current =>
            this.wrapped.Current;
    }
}

