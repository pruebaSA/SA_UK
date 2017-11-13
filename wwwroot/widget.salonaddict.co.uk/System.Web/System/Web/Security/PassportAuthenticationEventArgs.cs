namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PassportAuthenticationEventArgs : EventArgs
    {
        private HttpContext _Context;
        private PassportIdentity _Identity;
        private IPrincipal _User;

        public PassportAuthenticationEventArgs(PassportIdentity identity, HttpContext context)
        {
            this._Identity = identity;
            this._Context = context;
        }

        public HttpContext Context =>
            this._Context;

        public PassportIdentity Identity =>
            this._Identity;

        public IPrincipal User
        {
            get => 
                this._User;
            [SecurityPermission(SecurityAction.Demand, ControlPrincipal=true)]
            set
            {
                this._User = value;
            }
        }
    }
}

