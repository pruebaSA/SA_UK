namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class VarPar : XslNode
    {
        public XslFlags DefValueFlags;
        public QilNode Value;

        public VarPar(XslNodeType nt, QilName name, string select, XslVersion xslVer) : base(nt, name, select, xslVer)
        {
        }
    }
}

