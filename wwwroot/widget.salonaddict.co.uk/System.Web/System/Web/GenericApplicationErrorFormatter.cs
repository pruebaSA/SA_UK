namespace System.Web
{
    using System;

    internal class GenericApplicationErrorFormatter : ErrorFormatter
    {
        private bool _local;

        internal GenericApplicationErrorFormatter(bool local)
        {
            this._local = local;
        }

        internal override bool CanBeShownToAllUsers =>
            true;

        protected override string ColoredSquare2Content
        {
            get
            {
                string content = HttpUtility.HtmlEncode(System.Web.SR.GetString(this._local ? "Generic_Err_Local_Notes_Sample" : "Generic_Err_Remote_Notes_Sample"));
                return base.WrapWithLeftToRightTextFormatIfNeeded(content);
            }
        }

        protected override string ColoredSquare2Description
        {
            get
            {
                string str = HttpUtility.HtmlEncode(System.Web.SR.GetString("Generic_Err_Notes_Desc"));
                this.AdaptiveMiscContent.Add(str);
                return str;
            }
        }

        protected override string ColoredSquare2Title
        {
            get
            {
                string str = System.Web.SR.GetString("Generic_Err_Notes_Title");
                this.AdaptiveMiscContent.Add(str);
                return str;
            }
        }

        protected override string ColoredSquareContent
        {
            get
            {
                string content = HttpUtility.HtmlEncode(System.Web.SR.GetString(this._local ? "Generic_Err_Local_Details_Sample" : "Generic_Err_Remote_Details_Sample"));
                return base.WrapWithLeftToRightTextFormatIfNeeded(content);
            }
        }

        protected override string ColoredSquareDescription
        {
            get
            {
                string str = HttpUtility.HtmlEncode(System.Web.SR.GetString(this._local ? "Generic_Err_Local_Details_Desc" : "Generic_Err_Remote_Details_Desc"));
                this.AdaptiveMiscContent.Add(str);
                return str;
            }
        }

        protected override string ColoredSquareTitle
        {
            get
            {
                string str = System.Web.SR.GetString("Generic_Err_Details_Title");
                this.AdaptiveMiscContent.Add(str);
                return str;
            }
        }

        protected override string Description =>
            System.Web.SR.GetString(this._local ? "Generic_Err_Local_Desc" : "Generic_Err_Remote_Desc");

        protected override string ErrorTitle =>
            System.Web.SR.GetString("Generic_Err_Title");

        protected override string MiscSectionContent =>
            null;

        protected override string MiscSectionTitle =>
            null;

        protected override bool ShowSourceFileInfo =>
            false;
    }
}

