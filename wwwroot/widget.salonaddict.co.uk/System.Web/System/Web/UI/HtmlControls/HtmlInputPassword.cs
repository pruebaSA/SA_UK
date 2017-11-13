namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultEvent("ServerChange"), ValidationProperty("Value"), SupportsEventValidation, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlInputPassword : HtmlInputText, IPostBackDataHandler
    {
        private static readonly object EventServerChange = new object();

        public HtmlInputPassword() : base("password")
        {
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            this.ViewState.Remove("value");
            base.RenderAttributes(writer);
        }
    }
}

