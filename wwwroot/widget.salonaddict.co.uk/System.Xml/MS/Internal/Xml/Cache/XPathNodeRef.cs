namespace MS.Internal.Xml.Cache
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XPathNodeRef
    {
        private XPathNode[] page;
        private int idx;
        public static XPathNodeRef Null =>
            new XPathNodeRef();
        public XPathNodeRef(XPathNode[] page, int idx)
        {
            this.page = page;
            this.idx = idx;
        }

        public bool IsNull =>
            (this.page == null);
        public XPathNode[] Page =>
            this.page;
        public int Index =>
            this.idx;
        public override int GetHashCode() => 
            XPathNodeHelper.GetLocation(this.page, this.idx);
    }
}

