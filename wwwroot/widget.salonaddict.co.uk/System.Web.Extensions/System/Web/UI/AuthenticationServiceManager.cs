namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Script.Serialization;

    [TypeConverter(typeof(EmptyStringExpandableObjectConverter)), DefaultProperty("Path"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AuthenticationServiceManager
    {
        private string _path;

        internal static void ConfigureAuthenticationService(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, List<ScriptManagerProxy> proxies)
        {
            string relativeUrl = null;
            if (scriptManager.HasAuthenticationServiceManager)
            {
                relativeUrl = scriptManager.AuthenticationService.Path.Trim();
                if (relativeUrl.Length > 0)
                {
                    relativeUrl = scriptManager.ResolveUrl(relativeUrl);
                }
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    if (proxy.HasAuthenticationServiceManager)
                    {
                        relativeUrl = ApplicationServiceManager.MergeServiceUrls(proxy.AuthenticationService.Path, relativeUrl, proxy);
                    }
                }
            }
            GenerateInitializationScript(ref sb, context, scriptManager, relativeUrl);
        }

        private static void GenerateInitializationScript(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, string serviceUrl)
        {
            bool authenticationServiceEnabled = ApplicationServiceHelper.AuthenticationServiceEnabled;
            if (authenticationServiceEnabled)
            {
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                string str = scriptManager.ResolveClientUrl("~/Authentication_JSON_AppService.axd");
                sb.Append("Sys.Services._AuthenticationService.DefaultWebServicePath = '");
                sb.Append(JavaScriptString.QuoteString(str));
                sb.Append("';\n");
            }
            bool flag2 = !string.IsNullOrEmpty(serviceUrl);
            if (flag2)
            {
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                sb.Append("Sys.Services.AuthenticationService.set_path('");
                sb.Append(JavaScriptString.QuoteString(serviceUrl));
                sb.Append("');\n");
            }
            if ((authenticationServiceEnabled || flag2) && ((context != null) && context.Request.IsAuthenticated))
            {
                sb.Append("Sys.Services.AuthenticationService._setAuthenticated(true);\n");
            }
        }

        [Category("Behavior"), DefaultValue(""), UrlProperty, NotifyParentProperty(true), ResourceDescription("ApplicationServiceManager_Path")]
        public string Path
        {
            get => 
                (this._path ?? string.Empty);
            set
            {
                this._path = value;
            }
        }
    }
}

