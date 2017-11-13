namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebResourceAttribute : Attribute
    {
        private string _contentType;
        private bool _performSubstitution;
        private string _webResource;

        public WebResourceAttribute(string webResource, string contentType)
        {
            if (string.IsNullOrEmpty(webResource))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("webResource");
            }
            if (string.IsNullOrEmpty(contentType))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("contentType");
            }
            this._contentType = contentType;
            this._webResource = webResource;
            this._performSubstitution = false;
        }

        public string ContentType =>
            this._contentType;

        public bool PerformSubstitution
        {
            get => 
                this._performSubstitution;
            set
            {
                this._performSubstitution = value;
            }
        }

        public string WebResource =>
            this._webResource;
    }
}

