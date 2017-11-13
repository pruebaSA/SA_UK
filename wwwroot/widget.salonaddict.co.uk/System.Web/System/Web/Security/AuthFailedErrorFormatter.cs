namespace System.Web.Security
{
    using System;
    using System.Web;

    internal class AuthFailedErrorFormatter : ErrorFormatter
    {
        private static string _strErrorText;
        private static object _syncObject = new object();

        internal AuthFailedErrorFormatter()
        {
        }

        internal static string GetErrorText()
        {
            if (_strErrorText == null)
            {
                lock (_syncObject)
                {
                    if (_strErrorText == null)
                    {
                        _strErrorText = new AuthFailedErrorFormatter().GetErrorMessage();
                    }
                }
            }
            return _strErrorText;
        }

        protected override string ColoredSquareContent =>
            null;

        protected override string ColoredSquareTitle =>
            null;

        protected override string Description =>
            System.Web.SR.GetString("Assess_Denied_Description1");

        protected override string ErrorTitle =>
            System.Web.SR.GetString("Assess_Denied_Title");

        protected override string MiscSectionContent
        {
            get
            {
                string str = System.Web.SR.GetString("Assess_Denied_MiscContent1");
                this.AdaptiveMiscContent.Add(str);
                return str;
            }
        }

        protected override string MiscSectionTitle =>
            System.Web.SR.GetString("Assess_Denied_MiscTitle1");

        protected override bool ShowSourceFileInfo =>
            false;
    }
}

