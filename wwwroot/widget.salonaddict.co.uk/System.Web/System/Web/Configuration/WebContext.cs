namespace System.Web.Configuration
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebContext
    {
        private string _appConfigPath;
        private string _applicationPath;
        private string _locationSubPath;
        private string _path;
        private WebApplicationLevel _pathLevel;
        private string _site;

        public WebContext(WebApplicationLevel pathLevel, string site, string applicationPath, string path, string locationSubPath, string appConfigPath)
        {
            this._pathLevel = pathLevel;
            this._site = site;
            this._applicationPath = applicationPath;
            this._path = path;
            this._locationSubPath = locationSubPath;
            this._appConfigPath = appConfigPath;
        }

        public override string ToString() => 
            this._appConfigPath;

        public WebApplicationLevel ApplicationLevel =>
            this._pathLevel;

        public string ApplicationPath =>
            this._applicationPath;

        public string LocationSubPath =>
            this._locationSubPath;

        public string Path =>
            this._path;

        public string Site =>
            this._site;
    }
}

