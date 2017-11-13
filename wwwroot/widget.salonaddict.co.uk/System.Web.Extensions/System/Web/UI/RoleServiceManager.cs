namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Security;

    [DefaultProperty("Path"), TypeConverter(typeof(EmptyStringExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RoleServiceManager
    {
        private bool _loadRoles;
        private string _path;

        internal static void ConfigureRoleService(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, List<ScriptManagerProxy> proxies)
        {
            string relativeUrl = null;
            RoleServiceManager roleService;
            bool loadRoles = false;
            if (scriptManager.HasRoleServiceManager)
            {
                roleService = scriptManager.RoleService;
                loadRoles = roleService.LoadRoles;
                relativeUrl = roleService.Path.Trim();
                if (relativeUrl.Length > 0)
                {
                    relativeUrl = scriptManager.ResolveClientUrl(relativeUrl);
                }
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    if (proxy.HasRoleServiceManager)
                    {
                        roleService = proxy.RoleService;
                        if (roleService.LoadRoles)
                        {
                            loadRoles = true;
                        }
                        relativeUrl = ApplicationServiceManager.MergeServiceUrls(roleService.Path, relativeUrl, proxy);
                    }
                }
            }
            GenerateInitializationScript(ref sb, context, scriptManager, relativeUrl, loadRoles);
        }

        private static void GenerateInitializationScript(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, string serviceUrl, bool loadRoles)
        {
            bool roleServiceEnabled = ApplicationServiceHelper.RoleServiceEnabled;
            string str = null;
            if (roleServiceEnabled)
            {
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                str = scriptManager.ResolveClientUrl("~/Role_JSON_AppService.axd");
                sb.Append("Sys.Services._RoleService.DefaultWebServicePath = '");
                sb.Append(JavaScriptString.QuoteString(str));
                sb.Append("';\n");
            }
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                if (str == null)
                {
                    str = scriptManager.ResolveClientUrl("~/Role_JSON_AppService.axd");
                }
                if (loadRoles && !string.Equals(serviceUrl, str, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(AtlasWeb.RoleServiceManager_LoadRolesWithNonDefaultPath);
                }
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                sb.Append("Sys.Services.RoleService.set_path('");
                sb.Append(JavaScriptString.QuoteString(serviceUrl));
                sb.Append("');\n");
            }
            if (loadRoles)
            {
                string[] rolesForUser = Roles.GetRolesForUser();
                if ((rolesForUser != null) && (rolesForUser.Length > 0))
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(0x80);
                    }
                    sb.Append("Sys.Services.RoleService._roles = Sys.Serialization.JavaScriptSerializer.deserialize('");
                    sb.Append(JavaScriptString.QuoteString(JavaScriptSerializer.SerializeInternal(rolesForUser)));
                    sb.Append("');\n");
                }
            }
        }

        [DefaultValue(false), ResourceDescription("RoleServiceManager_LoadRoles"), Category("Behavior"), NotifyParentProperty(true)]
        public bool LoadRoles
        {
            get => 
                this._loadRoles;
            set
            {
                this._loadRoles = value;
            }
        }

        [Category("Behavior"), ResourceDescription("ApplicationServiceManager_Path"), DefaultValue(""), NotifyParentProperty(true), UrlProperty]
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

