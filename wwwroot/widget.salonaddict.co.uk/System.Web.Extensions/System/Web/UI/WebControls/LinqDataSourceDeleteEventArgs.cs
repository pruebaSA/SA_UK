namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceDeleteEventArgs : CancelEventArgs
    {
        private LinqDataSourceValidationException _exception;
        private bool _exceptionHandled;
        private object _originalObject;

        public LinqDataSourceDeleteEventArgs(object originalObject)
        {
            this._originalObject = originalObject;
        }

        public LinqDataSourceDeleteEventArgs(LinqDataSourceValidationException exception)
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

        public object OriginalObject =>
            this._originalObject;
    }
}

