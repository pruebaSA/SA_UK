namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceStatusEventArgs : EventArgs
    {
        private System.Exception _exception;
        private bool _exceptionHandled;
        private object _result;
        private int _totalRowCount;

        public LinqDataSourceStatusEventArgs(System.Exception exception)
        {
            this._totalRowCount = -1;
            this._exception = exception;
        }

        public LinqDataSourceStatusEventArgs(object result)
        {
            this._totalRowCount = -1;
            this._result = result;
        }

        public LinqDataSourceStatusEventArgs(object result, int totalRowCount)
        {
            this._totalRowCount = -1;
            this._result = result;
            this._totalRowCount = totalRowCount;
        }

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

        public object Result =>
            this._result;

        public int TotalRowCount =>
            this._totalRowCount;
    }
}

