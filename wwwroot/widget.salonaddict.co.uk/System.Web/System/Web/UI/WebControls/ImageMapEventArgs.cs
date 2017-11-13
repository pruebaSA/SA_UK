namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ImageMapEventArgs : EventArgs
    {
        private string _postBackValue;

        public ImageMapEventArgs(string value)
        {
            this._postBackValue = value;
        }

        public string PostBackValue =>
            this._postBackValue;
    }
}

