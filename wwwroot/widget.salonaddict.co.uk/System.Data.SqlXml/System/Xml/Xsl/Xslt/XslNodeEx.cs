namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XslNodeEx : XslNode
    {
        public readonly ISourceLineInfo ElemNameLi;
        public readonly ISourceLineInfo EndTagLi;

        public XslNodeEx(XslNodeType t, QilName name, object arg, XslVersion xslVer) : base(t, name, arg, xslVer)
        {
        }

        public XslNodeEx(XslNodeType t, QilName name, object arg, XsltInput.ContextInfo ctxInfo, XslVersion xslVer) : base(t, name, arg, xslVer)
        {
            this.ElemNameLi = ctxInfo.elemNameLi;
            this.EndTagLi = ctxInfo.endTagLi;
        }
    }
}

