namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DefaultAuthenticationEventArgs : EventArgs
    {
        private HttpContext _Context;

        public DefaultAuthenticationEventArgs(HttpContext context)
        {
            this._Context = context;
        }

        public HttpContext Context =>
            this._Context;
    }
}

