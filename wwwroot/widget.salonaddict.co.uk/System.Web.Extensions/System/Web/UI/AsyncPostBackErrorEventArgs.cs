namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AsyncPostBackErrorEventArgs : EventArgs
    {
        private readonly System.Exception _exception;

        public AsyncPostBackErrorEventArgs(System.Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            this._exception = exception;
        }

        public System.Exception Exception =>
            this._exception;
    }
}

