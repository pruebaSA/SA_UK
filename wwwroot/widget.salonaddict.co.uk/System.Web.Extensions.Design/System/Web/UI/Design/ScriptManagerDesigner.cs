namespace System.Web.UI.Design
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Web.ApplicationServices;
    using System.Web.Script.Serialization;
    using System.Web.UI;

    public class ScriptManagerDesigner : ControlDesigner
    {
        private static ScriptReference CloneScriptReference(ScriptReference script) => 
            new ScriptReference(script.Name, script.Assembly) { 
                IgnoreScriptPath = script.IgnoreScriptPath,
                NotifyScriptLoaded = script.NotifyScriptLoaded,
                Path = script.Path,
                ResourceUICultures = script.ResourceUICultures,
                ScriptMode = script.ScriptMode,
                ClientUrlResolver = script.ClientUrlResolver
            };

        private static List<ScriptReference> CloneScriptReferences(List<ScriptReferenceBase> scripts)
        {
            List<ScriptReference> list = new List<ScriptReference>();
            foreach (ScriptReferenceBase base2 in scripts)
            {
                ScriptReference script = base2 as ScriptReference;
                if (script != null)
                {
                    list.Add(CloneScriptReference(script));
                }
                else
                {
                    CompositeScriptReference reference2 = base2 as CompositeScriptReference;
                    foreach (ScriptReference reference3 in reference2.Scripts)
                    {
                        reference3.ClientUrlResolver = reference2.ClientUrlResolver;
                        list.Add(CloneScriptReference(reference3));
                    }
                }
            }
            return list;
        }

        public static string GetApplicationServices(ScriptManager scriptManager, IEnumerable<ScriptManagerProxy> proxies)
        {
            if (scriptManager == null)
            {
                throw new ArgumentNullException("scriptManager");
            }
            SortedList<string, object> topLevelSettings = new SortedList<string, object>();
            SortedList<string, SortedList<string, object>> profileGroups = null;
            GetProfileConfigProperties(topLevelSettings, ref profileGroups);
            if (scriptManager.HasProfileServiceManager)
            {
                GetProfileLoadProperties(scriptManager.ProfileService, topLevelSettings, ref profileGroups);
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    if (proxy.HasProfileServiceManager)
                    {
                        GetProfileLoadProperties(proxy.ProfileService, topLevelSettings, ref profileGroups);
                    }
                }
            }
            return GetProfilePropertiesScript(topLevelSettings, profileGroups);
        }

        public override string GetDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();

        private static void GetProfileConfigProperties(SortedList<string, object> topLevelSettings, ref SortedList<string, SortedList<string, object>> profileGroups)
        {
            SortedList<string, object> list = new SortedList<string, object>();
            Dictionary<string, object> profileAllowedGet = ApplicationServiceHelper.ProfileAllowedGet;
            if (profileAllowedGet != null)
            {
                foreach (KeyValuePair<string, object> pair in profileAllowedGet)
                {
                    list[pair.Key] = null;
                }
            }
            Dictionary<string, object> profileAllowedSet = ApplicationServiceHelper.ProfileAllowedSet;
            if (profileAllowedSet != null)
            {
                foreach (KeyValuePair<string, object> pair2 in profileAllowedSet)
                {
                    list[pair2.Key] = null;
                }
            }
            foreach (string str in list.Keys)
            {
                ProfileServiceManager.GetSettingsProperty(null, str, topLevelSettings, ref profileGroups, false);
            }
        }

        private static void GetProfileLoadProperties(ProfileServiceManager profile, SortedList<string, object> topLevelSettings, ref SortedList<string, SortedList<string, object>> profileGroups)
        {
            if (profile.HasLoadProperties)
            {
                foreach (string str in profile.LoadProperties)
                {
                    ProfileServiceManager.GetSettingsProperty(null, str, topLevelSettings, ref profileGroups, false);
                }
            }
        }

        private static string GetProfilePropertiesScript(SortedList<string, object> topLevelSettings, SortedList<string, SortedList<string, object>> profileGroups)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in topLevelSettings.Keys)
            {
                builder.Append("Sys.Services.ProfileService.properties['");
                builder.Append(JavaScriptString.QuoteString(str));
                builder.Append("'] = null;\r\n");
            }
            if (profileGroups != null)
            {
                foreach (KeyValuePair<string, SortedList<string, object>> pair in profileGroups)
                {
                    string str2 = "Sys.Services.ProfileService.properties['" + JavaScriptString.QuoteString(pair.Key) + "']";
                    builder.Append(str2).Append(" = new Sys.Services.ProfileGroup();");
                    foreach (string str3 in pair.Value.Keys)
                    {
                        builder.Append(str2).Append("['");
                        builder.Append(JavaScriptString.QuoteString(str3));
                        builder.Append("'] = null;");
                    }
                }
            }
            return builder.ToString();
        }

        public static string GetScriptFromWebResource(Assembly assembly, string resourceName, CultureInfo culture)
        {
            string str;
            return ScriptResourceAttribute.GetScriptFromWebResourceInternal(assembly, resourceName, culture, false, true, out str);
        }

        public static ReadOnlyCollection<ScriptReference> GetScriptReferences(ScriptManager scriptManager, IEnumerable<ScriptManagerProxy> proxies)
        {
            if (scriptManager == null)
            {
                throw new ArgumentNullException("scriptManager");
            }
            List<ScriptReferenceBase> scripts = new List<ScriptReferenceBase>();
            scriptManager.AddScriptCollections(scripts, proxies);
            scriptManager.AddFrameworkScripts(scripts);
            List<ScriptReference> list3 = CloneScriptReferences(ScriptManager.RemoveDuplicates(scripts));
            ResolvePaths(list3);
            return new ReadOnlyCollection<ScriptReference>(list3);
        }

        public static ReadOnlyCollection<ServiceReference> GetServiceReferences(ScriptManager scriptManager, IEnumerable<ScriptManagerProxy> proxies)
        {
            if (scriptManager == null)
            {
                throw new ArgumentNullException("scriptManager");
            }
            SortedList<string, object> list = new SortedList<string, object>();
            foreach (ServiceReference reference in scriptManager.Services)
            {
                string str = scriptManager.ResolveClientUrl(reference.Path);
                list[str] = null;
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    foreach (ServiceReference reference2 in proxy.Services)
                    {
                        string str2 = proxy.ResolveClientUrl(reference2.Path);
                        list[str2] = null;
                    }
                }
            }
            List<ServiceReference> list2 = new List<ServiceReference>();
            foreach (string str3 in list.Keys)
            {
                list2.Add(new ServiceReference(str3));
            }
            return new ReadOnlyCollection<ServiceReference>(list2);
        }

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(ScriptManager));
            base.Initialize(component);
        }

        private static void ResolvePaths(List<ScriptReference> scripts)
        {
            foreach (ScriptReference reference in scripts)
            {
                reference.Path = reference.ClientUrlResolver.ResolveClientUrl(reference.Path);
            }
        }
    }
}

