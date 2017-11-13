namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml.Xsl.Qil;

    internal static class AstFactory
    {
        private static QilFactory f = new QilFactory();

        public static System.Xml.Xsl.Xslt.XslNode ApplyImports(QilName mode, Stylesheet sheet, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.ApplyImports, mode, sheet, xslVer);

        public static XslNodeEx ApplyTemplates(QilName mode) => 
            new XslNodeEx(XslNodeType.ApplyTemplates, mode, null, XslVersion.Version10);

        public static XslNodeEx ApplyTemplates(QilName mode, string select, XsltInput.ContextInfo ctxInfo, XslVersion xslVer) => 
            new XslNodeEx(XslNodeType.ApplyTemplates, mode, select, ctxInfo, xslVer);

        public static NodeCtor Attribute(string nameAvt, string nsAvt, XslVersion xslVer) => 
            new NodeCtor(XslNodeType.Attribute, nameAvt, nsAvt, xslVer);

        public static System.Xml.Xsl.Xslt.AttributeSet AttributeSet(QilName name) => 
            new System.Xml.Xsl.Xslt.AttributeSet(name, XslVersion.Version10);

        public static XslNodeEx CallTemplate(QilName name, XsltInput.ContextInfo ctxInfo) => 
            new XslNodeEx(XslNodeType.CallTemplate, name, null, ctxInfo, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode Choose() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Choose);

        public static System.Xml.Xsl.Xslt.XslNode Comment() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Comment);

        public static System.Xml.Xsl.Xslt.XslNode Copy() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Copy);

        public static System.Xml.Xsl.Xslt.XslNode CopyOf(string select, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.CopyOf, null, select, xslVer);

        public static NodeCtor Element(string nameAvt, string nsAvt, XslVersion xslVer) => 
            new NodeCtor(XslNodeType.Element, nameAvt, nsAvt, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode Error(string message) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Error, null, message, XslVersion.Version10);

        public static XslNodeEx ForEach(string select, XsltInput.ContextInfo ctxInfo, XslVersion xslVer) => 
            new XslNodeEx(XslNodeType.ForEach, null, select, ctxInfo, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode If(string test, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.If, null, test, xslVer);

        public static System.Xml.Xsl.Xslt.Key Key(QilName name, string match, string use, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.Key(name, match, use, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode List() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.List);

        public static System.Xml.Xsl.Xslt.XslNode LiteralAttribute(QilName name, string value, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.LiteralAttribute, name, value, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode LiteralElement(QilName name) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.LiteralElement, name, null, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode Message(bool term) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Message, null, term, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode Nop() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Nop);

        public static System.Xml.Xsl.Xslt.Number Number(NumberLevel level, string count, string from, string value, string format, string lang, string letterValue, string groupingSeparator, string groupingSize, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.Number(level, count, from, value, format, lang, letterValue, groupingSeparator, groupingSize, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode Otherwise() => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.Otherwise);

        public static System.Xml.Xsl.Xslt.XslNode PI(string name, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.PI, null, name, xslVer);

        public static QilName QName(string local) => 
            f.LiteralQName(local);

        public static QilName QName(string local, string uri, string prefix) => 
            f.LiteralQName(local, uri, prefix);

        public static System.Xml.Xsl.Xslt.Sort Sort(string select, string lang, string dataType, string order, string caseOrder, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.Sort(select, lang, dataType, order, caseOrder, xslVer);

        public static System.Xml.Xsl.Xslt.Template Template(QilName name, string match, QilName mode, double priority, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.Template(name, match, mode, priority, xslVer);

        public static System.Xml.Xsl.Xslt.XslNode Text(string data) => 
            new System.Xml.Xsl.Xslt.Text(data, SerializationHints.None, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode Text(string data, SerializationHints hints) => 
            new System.Xml.Xsl.Xslt.Text(data, hints, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode UseAttributeSet(QilName name) => 
            new System.Xml.Xsl.Xslt.XslNode(XslNodeType.UseAttributeSet, name, null, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.VarPar VarPar(XslNodeType nt, QilName name, string select, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.VarPar(nt, name, select, xslVer);

        public static System.Xml.Xsl.Xslt.VarPar WithParam(QilName name) => 
            VarPar(XslNodeType.WithParam, name, null, XslVersion.Version10);

        public static System.Xml.Xsl.Xslt.XslNode XslNode(XslNodeType nodeType, QilName name, string arg, XslVersion xslVer) => 
            new System.Xml.Xsl.Xslt.XslNode(nodeType, name, arg, xslVer);
    }
}

