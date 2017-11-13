namespace System.Web
{
    using System;
    using System.Collections.Specialized;

    internal class PageNotFoundErrorFormatter : ErrorFormatter
    {
        private StringCollection _adaptiveMiscContent = new StringCollection();
        protected string _htmlEncodedUrl;

        internal PageNotFoundErrorFormatter(string url)
        {
            this._htmlEncodedUrl = HttpUtility.HtmlEncode(url);
            this._adaptiveMiscContent.Add(this._htmlEncodedUrl);
        }

        protected override StringCollection AdaptiveMiscContent =>
            this._adaptiveMiscContent;

        internal override bool CanBeShownToAllUsers =>
            true;

        protected override string Description =>
            HttpUtility.FormatPlainTextAsHtml(System.Web.SR.GetString("NotFound_Http_404"));

        protected override string ErrorTitle =>
            System.Web.SR.GetString("NotFound_Resource_Not_Found");

        protected override string MiscSectionContent =>
            this._htmlEncodedUrl;

        protected override string MiscSectionTitle =>
            System.Web.SR.GetString("NotFound_Requested_Url");

        protected override bool ShowSourceFileInfo =>
            false;
    }
}

