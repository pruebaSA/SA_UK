namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RepeaterItemEventArgs : EventArgs
    {
        private RepeaterItem item;

        public RepeaterItemEventArgs(RepeaterItem item)
        {
            this.item = item;
        }

        public RepeaterItem Item =>
            this.item;
    }
}

