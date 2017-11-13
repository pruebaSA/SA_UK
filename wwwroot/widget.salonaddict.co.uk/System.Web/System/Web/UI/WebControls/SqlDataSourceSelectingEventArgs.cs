namespace System.Web.UI.WebControls
{
    using System;
    using System.Data.Common;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SqlDataSourceSelectingEventArgs : SqlDataSourceCommandEventArgs
    {
        private DataSourceSelectArguments _arguments;

        public SqlDataSourceSelectingEventArgs(DbCommand command, DataSourceSelectArguments arguments) : base(command)
        {
            this._arguments = arguments;
        }

        public DataSourceSelectArguments Arguments =>
            this._arguments;
    }
}

