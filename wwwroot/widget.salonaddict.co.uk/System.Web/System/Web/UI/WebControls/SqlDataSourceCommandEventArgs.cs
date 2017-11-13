namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SqlDataSourceCommandEventArgs : CancelEventArgs
    {
        private DbCommand _command;

        public SqlDataSourceCommandEventArgs(DbCommand command)
        {
            this._command = command;
        }

        public DbCommand Command =>
            this._command;
    }
}

