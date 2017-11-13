namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false), AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ComboBoxButton : WebControl
    {
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Button;
    }
}

