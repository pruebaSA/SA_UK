namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridSortCommandEventArgs : EventArgs
    {
        private object commandSource;
        private string sortExpression;

        public DataGridSortCommandEventArgs(object commandSource, DataGridCommandEventArgs dce)
        {
            this.commandSource = commandSource;
            this.sortExpression = (string) dce.CommandArgument;
        }

        public object CommandSource =>
            this.commandSource;

        public string SortExpression =>
            this.sortExpression;
    }
}

