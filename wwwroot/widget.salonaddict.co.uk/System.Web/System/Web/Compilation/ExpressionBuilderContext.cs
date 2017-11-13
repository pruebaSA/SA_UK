namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ExpressionBuilderContext
    {
        private System.Web.UI.TemplateControl _templateControl;
        private System.Web.VirtualPath _virtualPath;

        public ExpressionBuilderContext(string virtualPath)
        {
            this._virtualPath = System.Web.VirtualPath.Create(virtualPath);
        }

        public ExpressionBuilderContext(System.Web.UI.TemplateControl templateControl)
        {
            this._templateControl = templateControl;
        }

        internal ExpressionBuilderContext(System.Web.VirtualPath virtualPath)
        {
            this._virtualPath = virtualPath;
        }

        public System.Web.UI.TemplateControl TemplateControl =>
            this._templateControl;

        public string VirtualPath
        {
            get
            {
                if ((this._virtualPath == null) && (this._templateControl != null))
                {
                    return this._templateControl.AppRelativeVirtualPath;
                }
                return System.Web.VirtualPath.GetVirtualPathString(this._virtualPath);
            }
        }

        internal System.Web.VirtualPath VirtualPathObject
        {
            get
            {
                if ((this._virtualPath == null) && (this._templateControl != null))
                {
                    return this._templateControl.VirtualPath;
                }
                return this._virtualPath;
            }
        }
    }
}

