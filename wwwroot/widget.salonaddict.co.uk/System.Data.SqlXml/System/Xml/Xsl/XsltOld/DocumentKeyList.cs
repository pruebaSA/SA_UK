namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential)]
    internal struct DocumentKeyList
    {
        private XPathNavigator rootNav;
        private Hashtable keyTable;
        public DocumentKeyList(XPathNavigator rootNav, Hashtable keyTable)
        {
            this.rootNav = rootNav;
            this.keyTable = keyTable;
        }

        public XPathNavigator RootNav =>
            this.rootNav;
        public Hashtable KeyTable =>
            this.keyTable;
    }
}

