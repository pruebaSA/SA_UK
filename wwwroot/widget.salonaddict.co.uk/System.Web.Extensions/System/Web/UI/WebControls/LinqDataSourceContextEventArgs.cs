namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceContextEventArgs : EventArgs
    {
        private object _objectInstance;
        private DataSourceOperation _operation;

        public LinqDataSourceContextEventArgs()
        {
            this._operation = DataSourceOperation.Select;
        }

        public LinqDataSourceContextEventArgs(DataSourceOperation operation)
        {
            this._operation = operation;
        }

        public object ObjectInstance
        {
            get => 
                this._objectInstance;
            set
            {
                this._objectInstance = value;
            }
        }

        public DataSourceOperation Operation =>
            this._operation;
    }
}

