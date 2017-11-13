namespace System.Web
{
    using System;
    using System.Collections.Specialized;

    internal class ParseErrorFormatter : FormatterWithFileInfo
    {
        private StringCollection _adaptiveMiscContent;
        private HttpParseException _excep;
        protected string _message;

        internal ParseErrorFormatter(HttpParseException e, string virtualPath, string sourceCode, int line, string message) : base(virtualPath, null, sourceCode, line)
        {
            this._adaptiveMiscContent = new StringCollection();
            this._excep = e;
            this._message = HttpUtility.FormatPlainTextAsHtml(message);
            this._adaptiveMiscContent.Add(this._message);
        }

        protected override StringCollection AdaptiveMiscContent =>
            this._adaptiveMiscContent;

        protected override string ColoredSquareTitle =>
            System.Web.SR.GetString("Parser_Source_Error");

        protected override string Description =>
            System.Web.SR.GetString("Parser_Desc");

        protected override string ErrorTitle =>
            System.Web.SR.GetString("Parser_Error");

        protected override System.Exception Exception =>
            this._excep;

        protected override string MiscSectionContent =>
            this._message;

        protected override string MiscSectionTitle =>
            System.Web.SR.GetString("Parser_Error_Message");
    }
}

