namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XmlNavigatorStack
    {
        private const int InitialStackSize = 8;
        private XPathNavigator[] stkNav;
        private int sp;
        public void Push(XPathNavigator nav)
        {
            if (this.stkNav == null)
            {
                this.stkNav = new XPathNavigator[8];
            }
            else if (this.sp >= this.stkNav.Length)
            {
                XPathNavigator[] stkNav = this.stkNav;
                this.stkNav = new XPathNavigator[2 * this.sp];
                Array.Copy(stkNav, this.stkNav, this.sp);
            }
            this.stkNav[this.sp++] = nav;
        }

        public XPathNavigator Pop() => 
            this.stkNav[--this.sp];

        public XPathNavigator Peek() => 
            this.stkNav[this.sp - 1];

        public void Reset()
        {
            this.sp = 0;
        }

        public bool IsEmpty =>
            (this.sp == 0);
    }
}

