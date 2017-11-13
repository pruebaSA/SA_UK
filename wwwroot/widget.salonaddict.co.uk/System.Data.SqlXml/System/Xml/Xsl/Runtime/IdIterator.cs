namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct IdIterator
    {
        private XPathNavigator navCurrent;
        private string[] idrefs;
        private int idx;
        public void Create(XPathNavigator context, string value)
        {
            this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, context);
            this.idrefs = XmlConvert.SplitString(value);
            this.idx = -1;
        }

        public bool MoveNext()
        {
            do
            {
                this.idx++;
                if (this.idx >= this.idrefs.Length)
                {
                    return false;
                }
            }
            while (!this.navCurrent.MoveToId(this.idrefs[this.idx]));
            return true;
        }

        public XPathNavigator Current =>
            this.navCurrent;
    }
}

