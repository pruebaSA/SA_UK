namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SiteMapNodeItemEventArgs : EventArgs
    {
        private SiteMapNodeItem _item;

        public SiteMapNodeItemEventArgs(SiteMapNodeItem item)
        {
            this._item = item;
        }

        public SiteMapNodeItem Item =>
            this._item;
    }
}

