namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartDisplayModeCancelEventArgs : CancelEventArgs
    {
        private WebPartDisplayMode _newDisplayMode;

        public WebPartDisplayModeCancelEventArgs(WebPartDisplayMode newDisplayMode)
        {
            this._newDisplayMode = newDisplayMode;
        }

        public WebPartDisplayMode NewDisplayMode
        {
            get => 
                this._newDisplayMode;
            set
            {
                this._newDisplayMode = value;
            }
        }
    }
}

