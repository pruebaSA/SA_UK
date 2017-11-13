namespace System.Xml.Xsl
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StringPair
    {
        private string left;
        private string right;
        public StringPair(string left, string right)
        {
            this.left = left;
            this.right = right;
        }

        public string Left =>
            this.left;
        public string Right =>
            this.right;
    }
}

