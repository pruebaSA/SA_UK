namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Diagnostics;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;
    using System.Xml.Xsl.XPath;

    internal class XsltQilFactory : XPathQilFactory
    {
        public XsltQilFactory(QilFactory f, bool debug) : base(f, debug)
        {
        }

        [Conditional("DEBUG")]
        public void CheckQName(QilNode n)
        {
        }

        [Conditional("DEBUG")]
        public void CheckXsltType(QilNode n)
        {
            switch (n.XmlType.TypeCode)
            {
            }
        }

        public QilNode DefaultValueMarker() => 
            base.QName("default-value", "urn:schemas-microsoft-com:xslt-debug");

        public QilNode InvokeBaseUri(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("base-uri"), XsltMethods.BaseUri, XmlQueryTypeFactory.StringX, new QilNode[] { n });

        public QilNode InvokeCheckScriptNamespace(string nsUri) => 
            base.XsltInvokeEarlyBound(base.QName("register-script-namespace"), XsltMethods.CheckScriptNamespace, XmlQueryTypeFactory.IntX, new QilNode[] { base.String(nsUri) });

        public QilNode InvokeElementAvailable(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("element-available"), XsltMethods.ElementAvailable, XmlQueryTypeFactory.BooleanX, new QilNode[] { n });

        public QilNode InvokeEXslObjectType(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("exsl:object-type"), XsltMethods.EXslObjectType, XmlQueryTypeFactory.StringX, new QilNode[] { n });

        public QilNode InvokeFormatNumberDynamic(QilNode value, QilNode formatPicture, QilNode decimalFormatName, QilNode errorMessageName) => 
            base.XsltInvokeEarlyBound(base.QName("format-number-dynamic"), XsltMethods.FormatNumberDynamic, XmlQueryTypeFactory.StringX, new QilNode[] { value, formatPicture, decimalFormatName, errorMessageName });

        public QilNode InvokeFormatNumberStatic(QilNode value, QilNode decimalFormatIndex) => 
            base.XsltInvokeEarlyBound(base.QName("format-number-static"), XsltMethods.FormatNumberStatic, XmlQueryTypeFactory.StringX, new QilNode[] { value, decimalFormatIndex });

        public QilNode InvokeFunctionAvailable(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("function-available"), XsltMethods.FunctionAvailable, XmlQueryTypeFactory.BooleanX, new QilNode[] { n });

        public QilNode InvokeIsSameNodeSort(QilNode n1, QilNode n2) => 
            base.XsltInvokeEarlyBound(base.QName("is-same-node-sort"), XsltMethods.IsSameNodeSort, XmlQueryTypeFactory.BooleanX, new QilNode[] { n1, n2 });

        public QilNode InvokeLangToLcid(QilNode n, bool fwdCompat) => 
            base.XsltInvokeEarlyBound(base.QName("lang-to-lcid"), XsltMethods.LangToLcid, XmlQueryTypeFactory.IntX, new QilNode[] { n, base.Boolean(fwdCompat) });

        public QilNode InvokeMsFormatDateTime(QilNode datetime, QilNode format, QilNode lang, QilNode isDate) => 
            base.XsltInvokeEarlyBound(base.QName("ms:format-date-time"), XsltMethods.MSFormatDateTime, XmlQueryTypeFactory.StringX, new QilNode[] { datetime, format, lang, isDate });

        public QilNode InvokeMsLocalName(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("ms:local-name"), XsltMethods.MSLocalName, XmlQueryTypeFactory.StringX, new QilNode[] { n });

        public QilNode InvokeMsNamespaceUri(QilNode n, QilNode currentNode) => 
            base.XsltInvokeEarlyBound(base.QName("ms:namespace-uri"), XsltMethods.MSNamespaceUri, XmlQueryTypeFactory.StringX, new QilNode[] { n, currentNode });

        public QilNode InvokeMsNumber(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("ms:number"), XsltMethods.MSNumber, XmlQueryTypeFactory.DoubleX, new QilNode[] { n });

        public QilNode InvokeMsStringCompare(QilNode x, QilNode y, QilNode lang, QilNode options) => 
            base.XsltInvokeEarlyBound(base.QName("ms:string-compare"), XsltMethods.MSStringCompare, XmlQueryTypeFactory.DoubleX, new QilNode[] { x, y, lang, options });

        public QilNode InvokeMsUtc(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("ms:utc"), XsltMethods.MSUtc, XmlQueryTypeFactory.StringX, new QilNode[] { n });

        public QilNode InvokeNumberFormat(QilNode value, QilNode format, QilNode lang, QilNode letterValue, QilNode groupingSeparator, QilNode groupingSize) => 
            base.XsltInvokeEarlyBound(base.QName("number-format"), XsltMethods.NumberFormat, XmlQueryTypeFactory.StringX, new QilNode[] { value, format, lang, letterValue, groupingSeparator, groupingSize });

        public QilNode InvokeOnCurrentNodeChanged(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("on-current-node-changed"), XsltMethods.OnCurrentNodeChanged, XmlQueryTypeFactory.IntX, new QilNode[] { n });

        public QilNode InvokeOuterXml(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("outer-xml"), XsltMethods.OuterXml, XmlQueryTypeFactory.StringX, new QilNode[] { n });

        public QilNode InvokeRegisterDecimalFormat(DecimalFormatDecl format) => 
            base.XsltInvokeEarlyBound(base.QName("register-decimal-format"), XsltMethods.RegisterDecimalFormat, XmlQueryTypeFactory.IntX, new QilNode[] { base.QName(format.Name.Name, format.Name.Namespace), base.String(format.InfinitySymbol), base.String(format.NanSymbol), base.String(new string(format.Characters)) });

        public QilNode InvokeRegisterDecimalFormatter(QilNode formatPicture, DecimalFormatDecl format) => 
            base.XsltInvokeEarlyBound(base.QName("register-decimal-formatter"), XsltMethods.RegisterDecimalFormatter, XmlQueryTypeFactory.DoubleX, new QilNode[] { formatPicture, base.String(format.InfinitySymbol), base.String(format.NanSymbol), base.String(new string(format.Characters)) });

        public QilNode InvokeSystemProperty(QilNode n) => 
            base.XsltInvokeEarlyBound(base.QName("system-property"), XsltMethods.SystemProperty, XmlQueryTypeFactory.Choice(XmlQueryTypeFactory.DoubleX, XmlQueryTypeFactory.StringX), new QilNode[] { n });

        public QilNode IsDefaultValueMarker(QilNode n) => 
            base.IsType(n, XmlQueryTypeFactory.QNameX);
    }
}

