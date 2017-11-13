namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Profile;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls;

    [DefaultProperty("Path"), TypeConverter(typeof(System.Web.UI.EmptyStringExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProfileServiceManager
    {
        private string[] _loadProperties;
        private string _path;

        internal static void ConfigureProfileService(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, List<ScriptManagerProxy> proxies)
        {
            string relativeUrl = null;
            ArrayList existingProperties = null;
            ProfileServiceManager profileService;
            if (scriptManager.HasProfileServiceManager)
            {
                profileService = scriptManager.ProfileService;
                relativeUrl = profileService.Path.Trim();
                if (relativeUrl.Length > 0)
                {
                    relativeUrl = scriptManager.ResolveClientUrl(relativeUrl);
                }
                if (profileService.HasLoadProperties)
                {
                    existingProperties = new ArrayList(profileService._loadProperties);
                }
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    if (proxy.HasProfileServiceManager)
                    {
                        profileService = proxy.ProfileService;
                        relativeUrl = ApplicationServiceManager.MergeServiceUrls(profileService.Path, relativeUrl, proxy);
                        if (profileService.HasLoadProperties)
                        {
                            if (existingProperties == null)
                            {
                                existingProperties = new ArrayList(profileService._loadProperties);
                            }
                            else
                            {
                                existingProperties = MergeProperties(existingProperties, profileService._loadProperties);
                            }
                        }
                    }
                }
            }
            GenerateInitializationScript(ref sb, context, scriptManager, relativeUrl, existingProperties);
        }

        private static void GenerateInitializationScript(ref StringBuilder sb, HttpContext context, ScriptManager scriptManager, string serviceUrl, ArrayList loadedProperties)
        {
            string str = null;
            bool flag = (loadedProperties != null) && (loadedProperties.Count > 0);
            if (ApplicationServiceHelper.ProfileServiceEnabled)
            {
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                str = scriptManager.ResolveClientUrl("~/Profile_JSON_AppService.axd");
                sb.Append("Sys.Services._ProfileService.DefaultWebServicePath = '");
                sb.Append(JavaScriptString.QuoteString(str));
                sb.Append("';\n");
            }
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                if (str == null)
                {
                    str = scriptManager.ResolveClientUrl("~/Profile_JSON_AppService.axd");
                }
                if (flag && !string.Equals(serviceUrl, str, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(AtlasWeb.ProfileServiceManager_LoadProperitesWithNonDefaultPath);
                }
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                sb.Append("Sys.Services.ProfileService.set_path('");
                sb.Append(JavaScriptString.QuoteString(serviceUrl));
                sb.Append("');\n");
            }
            if (flag)
            {
                if (sb == null)
                {
                    sb = new StringBuilder(0x80);
                }
                SortedList<string, object> topLevelSettings = new SortedList<string, object>(loadedProperties.Count);
                SortedList<string, SortedList<string, object>> profileGroups = null;
                ProfileBase profile = context.Profile;
                foreach (string str2 in loadedProperties)
                {
                    GetSettingsProperty(profile, str2, topLevelSettings, ref profileGroups, true);
                }
                RenderProfileProperties(sb, topLevelSettings, profileGroups);
            }
        }

        internal static void GetSettingsProperty(ProfileBase profile, string fullPropertyName, SortedList<string, object> topLevelSettings, ref SortedList<string, SortedList<string, object>> profileGroups, bool ensureExists)
        {
            string str;
            string str2;
            SortedList<string, object> list;
            int index = fullPropertyName.IndexOf('.');
            if (index == -1)
            {
                str = null;
                str2 = fullPropertyName;
                list = topLevelSettings;
            }
            else
            {
                str = fullPropertyName.Substring(0, index);
                str2 = fullPropertyName.Substring(index + 1);
                if (profileGroups == null)
                {
                    profileGroups = new SortedList<string, SortedList<string, object>>();
                    list = new SortedList<string, object>();
                    profileGroups.Add(str, list);
                }
                else
                {
                    list = profileGroups[str];
                    if (list == null)
                    {
                        list = new SortedList<string, object>();
                        profileGroups.Add(str, list);
                    }
                }
            }
            bool flag = ProfileBase.Properties[fullPropertyName] != null;
            if (ensureExists && !flag)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.AppService_UnknownProfileProperty, new object[] { fullPropertyName }));
            }
            if (flag)
            {
                list[str2] = profile?[fullPropertyName];
            }
        }

        internal static ArrayList MergeProperties(ArrayList existingProperties, string[] newProperties)
        {
            foreach (string str in newProperties)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    string item = str.Trim();
                    if ((item.Length > 0) && !existingProperties.Contains(item))
                    {
                        existingProperties.Add(item);
                    }
                }
            }
            return existingProperties;
        }

        private static void RenderProfileProperties(StringBuilder sb, SortedList<string, object> topLevelSettings, SortedList<string, SortedList<string, object>> profileGroups)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            sb.Append("Sys.Services.ProfileService.properties = Sys.Serialization.JavaScriptSerializer.deserialize('");
            sb.Append(JavaScriptString.QuoteString(serializer.Serialize(topLevelSettings)));
            sb.Append("');\n");
            if (profileGroups != null)
            {
                foreach (KeyValuePair<string, SortedList<string, object>> pair in profileGroups)
                {
                    sb.Append("Sys.Services.ProfileService.properties.");
                    sb.Append(pair.Key);
                    sb.Append(" = new Sys.Services.ProfileGroup(Sys.Serialization.JavaScriptSerializer.deserialize('");
                    sb.Append(JavaScriptString.QuoteString(serializer.Serialize(pair.Value)));
                    sb.Append("'));\n");
                }
            }
        }

        internal bool HasLoadProperties =>
            ((this._loadProperties != null) && (this._loadProperties.Length > 0));

        [DefaultValue((string) null), NotifyParentProperty(true), ResourceDescription("ProfileServiceManager_LoadProperties"), Category("Behavior"), TypeConverter(typeof(StringArrayConverter))]
        public string[] LoadProperties
        {
            get
            {
                if (this._loadProperties == null)
                {
                    this._loadProperties = new string[0];
                }
                return (string[]) this._loadProperties.Clone();
            }
            set
            {
                if (value != null)
                {
                    value = (string[]) value.Clone();
                }
                this._loadProperties = value;
            }
        }

        [DefaultValue(""), UrlProperty, Category("Behavior"), NotifyParentProperty(true), ResourceDescription("ApplicationServiceManager_Path")]
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

