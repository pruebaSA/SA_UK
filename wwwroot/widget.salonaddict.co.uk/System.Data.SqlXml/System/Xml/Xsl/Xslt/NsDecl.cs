namespace System.Xml.Xsl.Xslt
{
    using System;

    internal class NsDecl
    {
        public readonly string NsUri;
        public readonly string Prefix;
        public readonly NsDecl Prev;

        public NsDecl(NsDecl prev, string prefix, string nsUri)
        {
            this.Prev = prev;
            this.Prefix = prefix;
            this.NsUri = nsUri;
        }
    }
}

