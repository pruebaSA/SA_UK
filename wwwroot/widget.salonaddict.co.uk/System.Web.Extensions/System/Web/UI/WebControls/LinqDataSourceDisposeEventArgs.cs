namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceDisposeEventArgs : CancelEventArgs
    {
        private object _objectInstance;

        public LinqDataSourceDisposeEventArgs(object instance)
        {
            this._objectInstance = instance;
        }

        public object ObjectInstance =>
            this._objectInstance;
    }
}

