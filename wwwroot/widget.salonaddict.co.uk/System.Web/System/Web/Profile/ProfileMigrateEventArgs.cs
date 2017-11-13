namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileMigrateEventArgs : EventArgs
    {
        private string _AnonymousId;
        private HttpContext _Context;

        public ProfileMigrateEventArgs(HttpContext context, string anonymousId)
        {
            this._Context = context;
            this._AnonymousId = anonymousId;
        }

        public string AnonymousID =>
            this._AnonymousId;

        public HttpContext Context =>
            this._Context;
    }
}

