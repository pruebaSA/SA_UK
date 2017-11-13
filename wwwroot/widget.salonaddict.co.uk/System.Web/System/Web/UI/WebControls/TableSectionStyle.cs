namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TableSectionStyle : Style
    {
        [WebCategory("Behavior"), DefaultValue(true), WebSysDescription("TableSectionStyle_Visible"), NotifyParentProperty(true)]
        public bool Visible
        {
            get
            {
                object obj2 = base.ViewState["Visible"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                base.ViewState["Visible"] = value;
            }
        }
    }
}

