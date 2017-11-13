namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ImageClickEventArgs : EventArgs
    {
        public int X;
        public int Y;

        public ImageClickEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

