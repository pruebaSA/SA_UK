namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.Script.Services;

    [DefaultProperty("Path"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ServiceReference
    {
        private bool _inlineScript;
        private string _path;

        public ServiceReference()
        {
        }

        public ServiceReference(string path)
        {
            this.Path = path;
        }

        private string GetInlineScript(Control containingControl, HttpContext context, bool debug)
        {
            if (RestHandlerFactory.IsRestMethodCall(context.Request))
            {
                return string.Empty;
            }
            string servicePath = this.GetServicePath(containingControl, false);
            try
            {
                servicePath = VirtualPathUtility.Combine(context.Request.FilePath, servicePath);
            }
            catch
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.WebService_InvalidInlineVirtualPath, new object[] { servicePath }));
            }
            return WebServiceClientProxyGenerator.GetInlineClientProxyScript(servicePath, context, debug);
        }

        private string GetProxyPath(Control containingControl, bool debug)
        {
            if (debug)
            {
                return (this.GetServicePath(containingControl, true) + "/jsdebug");
            }
            return (this.GetServicePath(containingControl, true) + "/js");
        }

        private string GetServicePath(Control containingControl, bool encodeSpaces)
        {
            string path = this.Path;
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException(AtlasWeb.ServiceReference_PathCannotBeEmpty);
            }
            if (encodeSpaces)
            {
                return containingControl.ResolveClientUrl(path);
            }
            return containingControl.ResolveUrl(path);
        }

        internal void Register(Control containingControl, HttpContext context, ScriptManager scriptManager, bool debug)
        {
            if (this.InlineScript)
            {
                RenderClientScriptBlock(this.GetInlineScript(containingControl, context, debug), scriptManager);
            }
            else
            {
                RegisterClientScriptInclude(this.GetProxyPath(containingControl, debug), scriptManager);
            }
        }

        private static void RegisterClientScriptInclude(string path, ScriptManager scriptManager)
        {
            scriptManager.RegisterClientScriptIncludeInternal(scriptManager, typeof(ScriptManager), path, path);
        }

        private static void RenderClientScriptBlock(string script, ScriptManager scriptManager)
        {
            scriptManager.RegisterClientScriptBlockInternal(scriptManager, typeof(ScriptManager), script, script, true);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Path))
            {
                return this.Path;
            }
            return base.GetType().Name;
        }

        [ResourceDescription("ServiceReference_InlineScript"), Category("Behavior"), DefaultValue(false)]
        public bool InlineScript
        {
            get => 
                this._inlineScript;
            set
            {
                this._inlineScript = value;
            }
        }

        [ResourceDescription("ServiceReference_Path"), DefaultValue(""), Category("Behavior"), UrlProperty]
        public string Path
        {
            get
            {
                if (this._path == null)
                {
                    return string.Empty;
                }
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }
    }
}

