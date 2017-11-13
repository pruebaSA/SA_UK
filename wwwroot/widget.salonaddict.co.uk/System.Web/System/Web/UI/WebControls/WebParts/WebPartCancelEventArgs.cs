﻿namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartCancelEventArgs : CancelEventArgs
    {
        private System.Web.UI.WebControls.WebParts.WebPart _webPart;

        public WebPartCancelEventArgs(System.Web.UI.WebControls.WebParts.WebPart webPart)
        {
            this._webPart = webPart;
        }

        public System.Web.UI.WebControls.WebParts.WebPart WebPart
        {
            get => 
                this._webPart;
            set
            {
                this._webPart = value;
            }
        }
    }
}

