namespace System.Web.UI.WebControls
{
    using System;
    using System.Data.Common;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SqlDataSourceStatusEventArgs : EventArgs
    {
        private int _affectedRows;
        private DbCommand _command;
        private System.Exception _exception;
        private bool _exceptionHandled;

        public SqlDataSourceStatusEventArgs(DbCommand command, int affectedRows, System.Exception exception)
        {
            this._command = command;
            this._affectedRows = affectedRows;
            this._exception = exception;
        }

        public int AffectedRows =>
            this._affectedRows;

        public DbCommand Command =>
            this._command;

        public System.Exception Exception =>
            this._exception;

        public bool ExceptionHandled
        {
            get => 
                this._exceptionHandled;
            set
            {
                this._exceptionHandled = value;
            }
        }
    }
}

