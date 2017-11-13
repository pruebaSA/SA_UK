namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MenuEventArgs : CommandEventArgs
    {
        private object _commandSource;
        private MenuItem _item;

        public MenuEventArgs(MenuItem item) : this(item, null, new CommandEventArgs(string.Empty, null))
        {
        }

        public MenuEventArgs(MenuItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._item = item;
            this._commandSource = commandSource;
        }

        public object CommandSource =>
            this._commandSource;

        public MenuItem Item =>
            this._item;
    }
}

