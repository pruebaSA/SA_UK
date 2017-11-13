namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceDisposingEventArgs : CancelEventArgs
    {
        private object _objectInstance;

        public ObjectDataSourceDisposingEventArgs(object objectInstance)
        {
            this._objectInstance = objectInstance;
        }

        public object ObjectInstance =>
            this._objectInstance;
    }
}

