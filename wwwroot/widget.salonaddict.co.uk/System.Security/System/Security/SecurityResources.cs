namespace System.Security
{
    using System;
    using System.Resources;

    internal static class SecurityResources
    {
        private static ResourceManager s_resMgr;

        internal static string GetResourceString(string key) => 
            s_resMgr?.GetString(key, null);
    }
}

