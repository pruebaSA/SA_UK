namespace System.Web.DynamicData
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DynamicValidatorEventArgs : EventArgs
    {
        private System.Exception _exception;
        private DynamicDataSourceOperation _operation;

        public DynamicValidatorEventArgs(System.Exception exception, DynamicDataSourceOperation operation)
        {
            this._exception = exception;
            this._operation = operation;
        }

        public System.Exception Exception =>
            this._exception;

        public DynamicDataSourceOperation Operation =>
            this._operation;
    }
}

