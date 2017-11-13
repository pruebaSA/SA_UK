namespace System.Web.UI
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.Security.Permissions;
    using System.Web.Configuration;

    internal sealed class AppLevelCompilationSectionCache : ICompilationSection
    {
        private bool? _debug;
        private static readonly AppLevelCompilationSectionCache _instance = new AppLevelCompilationSectionCache();

        private AppLevelCompilationSectionCache()
        {
        }

        [SecurityCritical, SecurityTreatAsSafe, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool GetDebugFromConfig()
        {
            CompilationSection webApplicationSection = (CompilationSection) WebConfigurationManager.GetWebApplicationSection("system.web/compilation");
            return webApplicationSection.Debug;
        }

        public bool Debug
        {
            get
            {
                if (!this._debug.HasValue)
                {
                    this._debug = new bool?(GetDebugFromConfig());
                }
                return this._debug.Value;
            }
        }

        public static AppLevelCompilationSectionCache Instance =>
            _instance;
    }
}

