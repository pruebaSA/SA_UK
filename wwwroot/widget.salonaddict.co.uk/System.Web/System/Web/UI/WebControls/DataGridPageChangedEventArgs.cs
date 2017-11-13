namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridPageChangedEventArgs : EventArgs
    {
        private object commandSource;
        private int newPageIndex;

        public DataGridPageChangedEventArgs(object commandSource, int newPageIndex)
        {
            this.commandSource = commandSource;
            this.newPageIndex = newPageIndex;
        }

        public object CommandSource =>
            this.commandSource;

        public int NewPageIndex =>
            this.newPageIndex;
    }
}

