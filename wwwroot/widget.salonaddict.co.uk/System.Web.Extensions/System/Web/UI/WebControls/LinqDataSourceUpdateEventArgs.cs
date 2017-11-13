namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceUpdateEventArgs : CancelEventArgs
    {
        private LinqDataSourceValidationException _exception;
        private bool _exceptionHandled;
        private object _newObject;
        private object _originalObject;

        public LinqDataSourceUpdateEventArgs(LinqDataSourceValidationException exception)
        {
            this._exception = exception;
        }

        public LinqDataSourceUpdateEventArgs(object originalObject, object newObject)
        {
            this._originalObject = originalObject;
            this._newObject = newObject;
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

        public object OriginalObject =>
            this._originalObject;
    }
}

