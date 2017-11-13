namespace System.Web
{
    using System;

    internal class SecurityErrorFormatter : UnhandledErrorFormatter
    {
        internal SecurityErrorFormatter(Exception e) : base(e)
        {
        }

        protected override string Description =>
            HttpUtility.FormatPlainTextAsHtml(System.Web.SR.GetString("Security_Err_Desc"));

        protected override string ErrorTitle =>
            System.Web.SR.GetString("Security_Err_Error");
    }
}

