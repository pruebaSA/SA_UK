namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileAutoSaveEventArgs : EventArgs
    {
        private HttpContext _Context;
        private bool _ContinueSave = true;

        public ProfileAutoSaveEventArgs(HttpContext context)
        {
            this._Context = context;
        }

        public HttpContext Context =>
            this._Context;

        public bool ContinueWithProfileAutoSave
        {
            get => 
                this._ContinueSave;
            set
            {
                this._ContinueSave = value;
            }
        }
    }
}

