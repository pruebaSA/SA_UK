namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class NamespaceDecl
    {
        private NamespaceDecl next;
        private string nsUri;
        private string prefix;
        private string prevDefaultNsUri;

        internal NamespaceDecl(string prefix, string nsUri, string prevDefaultNsUri, NamespaceDecl next)
        {
            this.Init(prefix, nsUri, prevDefaultNsUri, next);
        }

        internal void Init(string prefix, string nsUri, string prevDefaultNsUri, NamespaceDecl next)
        {
            this.prefix = prefix;
            this.nsUri = nsUri;
            this.prevDefaultNsUri = prevDefaultNsUri;
            this.next = next;
        }

        internal NamespaceDecl Next =>
            this.next;

        internal string Prefix =>
            this.prefix;

        internal string PrevDefaultNsUri =>
            this.prevDefaultNsUri;

        internal string Uri =>
            this.nsUri;
    }
}

