namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartAuthorizationEventArgs : EventArgs
    {
        private string _authorizationFilter;
        private bool _isAuthorized;
        private bool _isShared;
        private string _path;
        private System.Type _type;

        public WebPartAuthorizationEventArgs(System.Type type, string path, string authorizationFilter, bool isShared)
        {
            this._type = type;
            this._path = path;
            this._authorizationFilter = authorizationFilter;
            this._isShared = isShared;
            this._isAuthorized = true;
        }

        public string AuthorizationFilter =>
            this._authorizationFilter;

        public bool IsAuthorized
        {
            get => 
                this._isAuthorized;
            set
            {
                this._isAuthorized = value;
            }
        }

        public bool IsShared =>
            this._isShared;

        public string Path =>
            this._path;

        public System.Type Type =>
            this._type;
    }
}

