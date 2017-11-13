namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml.Xsl;

    internal class XmlILQueryEventArgs : XsltMessageEncounteredEventArgs
    {
        private string message;

        public XmlILQueryEventArgs(string message)
        {
            this.message = message;
        }

        public override string Message =>
            this.message;
    }
}

