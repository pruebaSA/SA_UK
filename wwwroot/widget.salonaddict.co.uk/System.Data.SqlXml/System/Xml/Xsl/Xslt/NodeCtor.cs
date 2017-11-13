namespace System.Xml.Xsl.Xslt
{
    using System;

    internal class NodeCtor : XslNode
    {
        public readonly string NameAvt;
        public readonly string NsAvt;

        public NodeCtor(XslNodeType nt, string nameAvt, string nsAvt, XslVersion xslVer) : base(nt, null, null, xslVer)
        {
            this.NameAvt = nameAvt;
            this.NsAvt = nsAvt;
        }
    }
}

