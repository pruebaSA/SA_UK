namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceInsertEventArgs : CancelEventArgs
    {
        private LinqDataSourceValidationException _exception;
        private bool _exceptionHandled;
        private object _newObject;

        public LinqDataSourceInsertEventArgs(object newObject)
        {
            this._newObject = newObject;
        }

        public LinqDataSourceInsertEventArgs(LinqDataSourceValidationException exception)
        {
            this._exception = exception;
        }

        public LinqDataSourceValidationException Exception =>
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

        public object NewObject =>
            this._newObject;
    }
}

