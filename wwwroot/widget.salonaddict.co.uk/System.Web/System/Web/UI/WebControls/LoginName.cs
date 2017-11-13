namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("FormatString"), Bindable(false), Designer("System.Web.UI.Design.WebControls.LoginNameDesigner,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LoginName : WebControl
    {
        private const string _defaultFormatString = "{0}";

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.UserName))
            {
                base.Render(writer);
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.UserName))
            {
                base.RenderBeginTag(writer);
            }
        }

        protected internal override void RenderContents(HtmlTextWriter writer)
        {
            string userName = this.UserName;
            if (!string.IsNullOrEmpty(userName))
            {
                userName = HttpUtility.HtmlEncode(userName);
                string formatString = this.FormatString;
                if (formatString.Length == 0)
                {
                    writer.Write(userName);
                }
                else
                {
                    try
                    {
                        writer.Write(string.Format(CultureInfo.CurrentCulture, formatString, new object[] { userName }));
                    }
                    catch (FormatException exception)
                    {
                        throw new FormatException(System.Web.SR.GetString("LoginName_InvalidFormatString"), exception);
                    }
                }
            }
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.UserName))
            {
                base.RenderEndTag(writer);
            }
        }

        [DefaultValue("{0}"), WebSysDescription("LoginName_FormatString"), WebCategory("Appearance"), Localizable(true)]
        public virtual string FormatString
        {
            get
            {
                object obj2 = this.ViewState["FormatString"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "{0}";
            }
            set
            {
                this.ViewState["FormatString"] = value;
            }
        }

        internal string UserName
        {
            get
            {
                if (base.DesignMode)
                {
                    return System.Web.SR.GetString("LoginName_DesignModeUserName");
                }
                return LoginUtil.GetUserName(this);
            }
        }
    }
}

