namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;

    internal class XsltOutput : CompiledAction
    {
        private Hashtable cdataElements;
        private string doctypePublic;
        private int doctypePublicSId = 0x7fffffff;
        private string doctypeSystem;
        private int doctypeSystemSId = 0x7fffffff;
        private System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        private int encodingSId = 0x7fffffff;
        private bool indent;
        private int indentSId = 0x7fffffff;
        private string mediaType = "text/html";
        private int mediaTypeSId = 0x7fffffff;
        private OutputMethod method = OutputMethod.Unknown;
        private int methodSId = 0x7fffffff;
        private bool omitXmlDecl;
        private int omitXmlDeclSId = 0x7fffffff;
        private bool standalone;
        private int standaloneSId = 0x7fffffff;
        private string version;
        private int versionSId = 0x7fffffff;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckEmpty(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string str2 = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Method))
            {
                if (compiler.Stylesheetid <= this.methodSId)
                {
                    this.method = ParseOutputMethod(str2, compiler);
                    this.methodSId = compiler.Stylesheetid;
                    if (this.indentSId == 0x7fffffff)
                    {
                        this.indent = this.method == OutputMethod.Html;
                    }
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Version))
            {
                if (compiler.Stylesheetid <= this.versionSId)
                {
                    this.version = str2;
                    this.versionSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Encoding))
            {
                if (compiler.Stylesheetid <= this.encodingSId)
                {
                    try
                    {
                        this.encoding = System.Text.Encoding.GetEncoding(str2);
                        this.encodingSId = compiler.Stylesheetid;
                    }
                    catch (NotSupportedException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.OmitXmlDeclaration))
            {
                if (compiler.Stylesheetid <= this.omitXmlDeclSId)
                {
                    this.omitXmlDecl = compiler.GetYesNo(str2);
                    this.omitXmlDeclSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Standalone))
            {
                if (compiler.Stylesheetid <= this.standaloneSId)
                {
                    this.standalone = compiler.GetYesNo(str2);
                    this.standaloneSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.DoctypePublic))
            {
                if (compiler.Stylesheetid <= this.doctypePublicSId)
                {
                    this.doctypePublic = str2;
                    this.doctypePublicSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.DoctypeSystem))
            {
                if (compiler.Stylesheetid <= this.doctypeSystemSId)
                {
                    this.doctypeSystem = str2;
                    this.doctypeSystemSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Indent))
            {
                if (compiler.Stylesheetid <= this.indentSId)
                {
                    this.indent = compiler.GetYesNo(str2);
                    this.indentSId = compiler.Stylesheetid;
                }
            }
            else if (Keywords.Equals(localName, compiler.Atoms.MediaType))
            {
                if (compiler.Stylesheetid <= this.mediaTypeSId)
                {
                    this.mediaType = str2;
                    this.mediaTypeSId = compiler.Stylesheetid;
                }
            }
            else
            {
                if (!Keywords.Equals(localName, compiler.Atoms.CdataSectionElements))
                {
                    return false;
                }
                string[] strArray = XmlConvert.SplitString(str2);
                if (this.cdataElements == null)
                {
                    this.cdataElements = new Hashtable(strArray.Length);
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    XmlQualifiedName name = compiler.CreateXmlQName(strArray[i]);
                    this.cdataElements[name] = name;
                }
            }
            return true;
        }

        internal XsltOutput CreateDerivedOutput(OutputMethod method)
        {
            XsltOutput output = (XsltOutput) base.MemberwiseClone();
            output.method = method;
            if ((method == OutputMethod.Html) && (this.indentSId == 0x7fffffff))
            {
                output.indent = true;
            }
            return output;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
        }

        private static OutputMethod ParseOutputMethod(string value, Compiler compiler)
        {
            XmlQualifiedName name = compiler.CreateXPathQName(value);
            if (name.Namespace.Length != 0)
            {
                return OutputMethod.Other;
            }
            switch (name.Name)
            {
                case "xml":
                    return OutputMethod.Xml;

                case "html":
                    return OutputMethod.Html;

                case "text":
                    return OutputMethod.Text;
            }
            if (!compiler.ForwardCompatibility)
            {
                throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "method", value });
            }
            return OutputMethod.Unknown;
        }

        internal Hashtable CDataElements =>
            this.cdataElements;

        internal string DoctypePublic =>
            this.doctypePublic;

        internal string DoctypeSystem =>
            this.doctypeSystem;

        internal System.Text.Encoding Encoding =>
            this.encoding;

        internal bool HasStandalone =>
            (this.standaloneSId != 0x7fffffff);

        internal bool Indent =>
            this.indent;

        internal string MediaType =>
            this.mediaType;

        internal OutputMethod Method =>
            this.method;

        internal bool OmitXmlDeclaration =>
            this.omitXmlDecl;

        internal bool Standalone =>
            this.standalone;

        internal enum OutputMethod
        {
            Xml,
            Html,
            Text,
            Other,
            Unknown
        }
    }
}

