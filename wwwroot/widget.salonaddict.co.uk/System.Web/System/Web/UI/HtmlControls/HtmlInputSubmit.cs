namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultEvent("ServerClick"), SupportsEventValidation, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlInputSubmit : HtmlInputButton, IPostBackEventHandler
    {
        public HtmlInputSubmit() : base("submit")
        {
        }

        public HtmlInputSubmit(string type) : base(type)
        {
        }

        internal override void RenderAttributesInternal(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                Util.WriteOnClickAttribute(writer, this, true, false, this.CausesValidation && (this.Page.GetValidators(this.ValidationGroup).Count > 0), this.ValidationGroup);
            }
        }
    }
}

