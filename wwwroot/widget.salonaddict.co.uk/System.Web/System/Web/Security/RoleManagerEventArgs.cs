namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RoleManagerEventArgs : EventArgs
    {
        private HttpContext _Context;
        private bool _RolesPopulated;

        public RoleManagerEventArgs(HttpContext context)
        {
            this._Context = context;
        }

        public HttpContext Context =>
            this._Context;

        public bool RolesPopulated
        {
            get => 
                this._RolesPopulated;
            set
            {
                this._RolesPopulated = value;
            }
        }
    }
}

