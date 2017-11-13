namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileEventArgs : EventArgs
    {
        private HttpContext _Context;
        private ProfileBase _Profile;

        public ProfileEventArgs(HttpContext context)
        {
            this._Context = context;
        }

        public HttpContext Context =>
            this._Context;

        public ProfileBase Profile
        {
            get => 
                this._Profile;
            set
            {
                this._Profile = value;
            }
        }
    }
}

