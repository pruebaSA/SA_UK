namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CommandEventArgs : EventArgs
    {
        private object argument;
        private string commandName;

        public CommandEventArgs(CommandEventArgs e) : this(e.CommandName, e.CommandArgument)
        {
        }

        public CommandEventArgs(string commandName, object argument)
        {
            this.commandName = commandName;
            this.argument = argument;
        }

        public object CommandArgument =>
            this.argument;

        public string CommandName =>
            this.commandName;
    }
}

