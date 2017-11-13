namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AnonymousIdentificationEventArgs : EventArgs
    {
        private string _AnonymousId;
        private HttpContext _Context;

        public AnonymousIdentificationEventArgs(HttpContext context)
        {
            this._Context = context;
        }

        public string AnonymousID
        {
            get => 
                this._AnonymousId;
            set
            {
                this._AnonymousId = value;
            }
        }

        public HttpContext Context =>
            this._Context;
    }
}

