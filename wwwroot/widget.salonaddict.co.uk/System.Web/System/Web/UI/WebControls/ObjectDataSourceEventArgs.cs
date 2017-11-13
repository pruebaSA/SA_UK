namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceEventArgs : EventArgs
    {
        private object _objectInstance;

        public ObjectDataSourceEventArgs(object objectInstance)
        {
            this._objectInstance = objectInstance;
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
    }
}

