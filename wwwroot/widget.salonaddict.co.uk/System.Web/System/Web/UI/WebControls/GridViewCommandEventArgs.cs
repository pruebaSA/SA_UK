namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewCommandEventArgs : CommandEventArgs
    {
        private object _commandSource;
        private GridViewRow _row;

        public GridViewCommandEventArgs(object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._commandSource = commandSource;
        }

        public GridViewCommandEventArgs(GridViewRow row, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._row = row;
            this._commandSource = commandSource;
        }

        public object CommandSource =>
            this._commandSource;

        internal GridViewRow Row =>
            this._row;
    }
}

