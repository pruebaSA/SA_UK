namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartEventArgs : EventArgs
    {
        private System.Web.UI.WebControls.WebParts.WebPart _webPart;

        public WebPartEventArgs(System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            this._webPart = webPart;
        }

        public System.Web.UI.WebControls.WebParts.WebPart WebPart =>
            this._webPart;
    }
}

