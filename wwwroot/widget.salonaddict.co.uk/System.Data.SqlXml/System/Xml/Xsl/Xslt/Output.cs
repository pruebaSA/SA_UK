namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml;

    internal class Output
    {
        public int DocTypePublicPrec = -2147483648;
        public int DocTypeSystemPrec = -2147483648;
        public string Encoding;
        public int EncodingPrec = -2147483648;
        public int IndentPrec = -2147483648;
        public int MediaTypePrec = -2147483648;
        public XmlQualifiedName Method;
        public int MethodPrec = -2147483648;
        public const int NeverDeclaredPrec = -2147483648;
        public int OmitXmlDeclarationPrec = -2147483648;
        public XmlWriterSettings Settings = new XmlWriterSettings();
        public int StandalonePrec = -2147483648;
        public string Version;
        public int VersionPrec = -2147483648;

        public Output()
        {
            this.Settings.OutputMethod = XmlOutputMethod.AutoDetect;
            this.Settings.AutoXmlDeclaration = true;
            this.Settings.ConformanceLevel = ConformanceLevel.Auto;
            this.Settings.MergeCDataSections = true;
        }
    }
}

