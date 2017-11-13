namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataPagerFieldCommandEventArgs : CommandEventArgs
    {
        private object _commandSource;
        private DataPagerFieldItem _item;

        public DataPagerFieldCommandEventArgs(DataPagerFieldItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._item = item;
            this._commandSource = commandSource;
        }

        public object CommandSource =>
            this._commandSource;

        public DataPagerFieldItem Item =>
            this._item;
    }
}

