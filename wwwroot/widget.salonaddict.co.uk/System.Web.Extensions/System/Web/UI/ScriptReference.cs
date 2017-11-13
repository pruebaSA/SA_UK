namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Handlers;
    using System.Web.Resources;
    using System.Web.Util;

    [DefaultProperty("Path"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptReference : ScriptReferenceBase
    {
        private string _assembly;
        private bool _ignoreScriptPath;
        private string _name;
        private static readonly Hashtable _scriptPathCache = Hashtable.Synchronized(new Hashtable());

        public ScriptReference()
        {
        }

        public ScriptReference(string path) : this()
        {
            base.Path = path;
        }

        public ScriptReference(string name, string assembly) : this()
        {
            this.Name = name;
            this.Assembly = assembly;
        }

        internal ScriptReference(string name, IClientUrlResolver clientUrlResolver, Control containingControl) : this()
        {
            this.Name = name;
            base.ClientUrlResolver = clientUrlResolver;
            base.IsStaticReference = true;
            base.ContainingControl = containingControl;
        }

        internal CultureInfo DetermineCulture()
        {
            if ((base.ResourceUICultures == null) || (base.ResourceUICultures.Length == 0))
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    return ScriptResourceHandler.DetermineNearestAvailableCulture(this.GetAssembly(), this.Name, CultureInfo.CurrentUICulture);
                }
                return CultureInfo.InvariantCulture;
            }
            CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
            while (!currentUICulture.Equals(CultureInfo.InvariantCulture))
            {
                string a = currentUICulture.ToString();
                foreach (string str2 in base.ResourceUICultures)
                {
                    if (string.Equals(a, str2.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return currentUICulture;
                    }
                }
                currentUICulture = currentUICulture.Parent;
            }
            return currentUICulture;
        }

        internal System.Reflection.Assembly GetAssembly()
        {
            string assembly = this.Assembly;
            if (string.IsNullOrEmpty(assembly))
            {
                return AssemblyCache.SystemWebExtensions;
            }
            return AssemblyCache.Load(assembly);
        }

        private static string GetDebugName(string releaseName)
        {
            if (!releaseName.EndsWith(".js", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, AtlasWeb.ScriptReference_InvalidReleaseScriptName, new object[] { releaseName }));
            }
            return ScriptReferenceBase.ReplaceExtension(releaseName);
        }

        internal string GetPath(string releasePath, bool isDebuggingEnabled)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return this.GetPathWithoutName(releasePath, isDebuggingEnabled);
            }
            return this.GetPathWithName(releasePath, isDebuggingEnabled);
        }

        private string GetPathWithName(string releasePath, bool isDebuggingEnabled)
        {
            if (this.ShouldUseDebugScript(this.Name, this.GetAssembly(), isDebuggingEnabled))
            {
                return ScriptReferenceBase.GetDebugPath(releasePath);
            }
            return releasePath;
        }

        private string GetPathWithoutName(string releasePath, bool isDebuggingEnabled)
        {
            if (isDebuggingEnabled)
            {
                return ScriptReferenceBase.GetDebugPath(releasePath);
            }
            return releasePath;
        }

        internal string GetResourceName(string releaseName, System.Reflection.Assembly assembly, bool isDebuggingEnabled)
        {
            if (this.ShouldUseDebugScript(releaseName, assembly, isDebuggingEnabled))
            {
                return GetDebugName(releaseName);
            }
            return releaseName;
        }

        internal static string GetScriptPath(string resourceName, System.Reflection.Assembly assembly, CultureInfo culture, string scriptPath) => 
            (scriptPath + "/" + GetScriptPathCached(resourceName, assembly, culture));

        private static string GetScriptPathCached(string resourceName, System.Reflection.Assembly assembly, CultureInfo culture)
        {
            Tuple tuple = new Tuple(new object[] { resourceName, assembly, culture });
            string str = (string) _scriptPathCache[tuple];
            if (str == null)
            {
                AssemblyName name = new AssemblyName(assembly.FullName);
                string str2 = name.Name;
                string str3 = name.Version.ToString();
                string assemblyFileVersion = AssemblyUtil.GetAssemblyFileVersion(assembly);
                if (!culture.Equals(CultureInfo.InvariantCulture) && resourceName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = resourceName.Substring(0, resourceName.Length - 3) + "." + culture.Name + ".js";
                }
                str = string.Join("/", new string[] { HttpUtility.UrlEncode(str2), str3, HttpUtility.UrlEncode(assemblyFileVersion), HttpUtility.UrlEncode(resourceName) });
                _scriptPathCache[tuple] = str;
            }
            return str;
        }

        protected internal override string GetUrl(ScriptManager scriptManager, bool zip)
        {
            bool flag = !string.IsNullOrEmpty(this.Name);
            bool flag2 = !string.IsNullOrEmpty(this.Assembly);
            bool flag3 = !string.IsNullOrEmpty(base.Path);
            if (!flag && !flag3)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptReference_NameAndPathCannotBeEmpty);
            }
            if (flag2 && !flag)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptReference_AssemblyRequiresName);
            }
            if (flag3)
            {
                return this.GetUrlFromPath(scriptManager);
            }
            return this.GetUrlFromName(scriptManager, scriptManager.Control, zip);
        }

        private string GetUrlFromName(ScriptManager scriptManager, IControl scriptManagerControl, bool zip)
        {
            string name = this.Name;
            System.Reflection.Assembly assembly = this.GetAssembly();
            string resourceName = this.GetResourceName(name, assembly, this.IsDebuggingEnabled(scriptManager));
            CultureInfo culture = scriptManager.EnableScriptLocalization ? this.DetermineCulture() : CultureInfo.InvariantCulture;
            if (this.IgnoreScriptPath || string.IsNullOrEmpty(scriptManager.ScriptPath))
            {
                return ScriptResourceHandler.GetScriptResourceUrl(assembly, resourceName, culture, zip, base.NotifyScriptLoaded);
            }
            string relativeUrl = GetScriptPath(resourceName, assembly, culture, scriptManager.ScriptPath);
            return scriptManagerControl.ResolveClientUrl(relativeUrl);
        }

        private string GetUrlFromPath(ScriptManager scriptManager)
        {
            string path = base.Path;
            string relativeUrl = this.GetPath(path, this.IsDebuggingEnabled(scriptManager));
            if (scriptManager.EnableScriptLocalization)
            {
                CultureInfo info = this.DetermineCulture();
                if (!info.Equals(CultureInfo.InvariantCulture))
                {
                    relativeUrl = relativeUrl.Substring(0, relativeUrl.Length - 2) + info.ToString() + ".js";
                }
            }
            return base.ClientUrlResolver.ResolveClientUrl(relativeUrl);
        }

        private bool IsDebuggingEnabled(ScriptManager scriptManager)
        {
            if (!scriptManager.DeploymentSectionRetail)
            {
                switch (this.EffectiveScriptMode)
                {
                    case ScriptMode.Inherit:
                        return scriptManager.IsDebuggingEnabled;

                    case ScriptMode.Debug:
                        return true;

                    case ScriptMode.Release:
                        return false;
                }
            }
            return false;
        }

        protected internal override bool IsFromSystemWebExtensions() => 
            (this.GetAssembly() == AssemblyCache.SystemWebExtensions);

        internal bool ShouldUseDebugScript(string releaseName, System.Reflection.Assembly assembly, bool isDebuggingEnabled)
        {
            bool flag;
            string resourceName = null;
            if (isDebuggingEnabled)
            {
                resourceName = GetDebugName(releaseName);
                if ((base.ScriptMode == ScriptMode.Auto) && !WebResourceUtil.AssemblyContainsWebResource(assembly, resourceName))
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            else
            {
                flag = false;
            }
            WebResourceUtil.VerifyAssemblyContainsReleaseWebResource(assembly, releaseName);
            if (flag)
            {
                WebResourceUtil.VerifyAssemblyContainsDebugWebResource(assembly, resourceName);
            }
            return flag;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                return this.Name;
            }
            if (!string.IsNullOrEmpty(base.Path))
            {
                return base.Path;
            }
            return base.GetType().Name;
        }

        [Category("Behavior"), DefaultValue(""), ResourceDescription("ScriptReference_Assembly")]
        public string Assembly
        {
            get
            {
                if (this._assembly != null)
                {
                    return this._assembly;
                }
                return string.Empty;
            }
            set
            {
                this._assembly = value;
            }
        }

        internal ScriptMode EffectiveScriptMode
        {
            get
            {
                if (base.ScriptMode != ScriptMode.Auto)
                {
                    return base.ScriptMode;
                }
                if (!string.IsNullOrEmpty(this.Name))
                {
                    return ScriptMode.Inherit;
                }
                return ScriptMode.Release;
            }
        }

        [Category("Behavior"), ResourceDescription("ScriptReference_IgnoreScriptPath"), DefaultValue(false)]
        public bool IgnoreScriptPath
        {
            get => 
                this._ignoreScriptPath;
            set
            {
                this._ignoreScriptPath = value;
            }
        }

        [ResourceDescription("ScriptReference_Name"), Category("Behavior"), DefaultValue("")]
        public string Name
        {
            get
            {
                if (this._name != null)
                {
                    return this._name;
                }
                return string.Empty;
            }
            set
            {
                this._name = value;
            }
        }
    }
}

