namespace System.Web.Security
{
    using System;
    using System.Web;

    internal class PassportAuthFailedErrorFormatter : ErrorFormatter
    {
        protected override string ColoredSquareContent =>
            null;

        protected override string ColoredSquareTitle =>
            null;

        protected override string Description =>
            System.Web.SR.GetString("PassportAuthFailed_Description");

        protected override string ErrorTitle =>
            System.Web.SR.GetString("PassportAuthFailed_Title");

        protected override string MiscSectionContent =>
            null;

        protected override string MiscSectionTitle =>
            System.Web.SR.GetString("Assess_Denied_Title");

        protected override bool ShowSourceFileInfo =>
            false;
    }
}

