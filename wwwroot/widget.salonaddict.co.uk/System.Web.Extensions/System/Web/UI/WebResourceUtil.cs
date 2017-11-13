namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Resources;
    using System.Web.Util;

    internal static class WebResourceUtil
    {
        private static readonly Hashtable _assemblyContainsWebResourceCache = Hashtable.Synchronized(new Hashtable());

        public static bool AssemblyContainsWebResource(Assembly assembly, string resourceName)
        {
            if (assembly == AssemblyCache.SystemWebExtensions)
            {
                return SystemWebExtensionsContainsWebResource(resourceName);
            }
            Pair<string, Assembly> pair = new Pair<string, Assembly>(resourceName, assembly);
            object obj2 = _assemblyContainsWebResourceCache[pair];
            if (obj2 == null)
            {
                obj2 = false;
                foreach (WebResourceAttribute attribute in assembly.GetCustomAttributes(typeof(WebResourceAttribute), false))
                {
                    if (string.Equals(attribute.WebResource, resourceName, StringComparison.Ordinal))
                    {
                        if (assembly.GetManifestResourceStream(resourceName) == null)
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, AtlasWeb.WebResourceUtil_AssemblyDoesNotContainEmbeddedResource, new object[] { assembly, resourceName }));
                        }
                        obj2 = true;
                        break;
                    }
                }
                _assemblyContainsWebResourceCache[pair] = obj2;
            }
            return (bool) obj2;
        }

        private static bool SystemWebExtensionsContainsWebResource(string resourceName)
        {
            switch (resourceName.Length)
            {
                case 0x15:
                    return (resourceName == "MicrosoftAjaxTimer.js");

                case 0x16:
                    return (resourceName == "MicrosoftAjax.debug.js");

                case 0x18:
                    return (resourceName == "MicrosoftAjaxWebForms.js");

                case 0x1b:
                    return ((resourceName == "MicrosoftAjaxTimer.debug.js") || (resourceName == "MicrosoftAjaxDataService.js"));

                case 0x10:
                    return (resourceName == "MicrosoftAjax.js");

                case 30:
                    return (resourceName == "MicrosoftAjaxWebForms.debug.js");

                case 0x21:
                    return (resourceName == "MicrosoftAjaxDataService.debug.js");
            }
            return false;
        }

        public static void VerifyAssemblyContainsDebugWebResource(Assembly assembly, string debugResourceName)
        {
            if (!AssemblyContainsWebResource(assembly, debugResourceName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, AtlasWeb.WebResourceUtil_AssemblyDoesNotContainDebugWebResource, new object[] { assembly, debugResourceName }));
            }
        }

        public static void VerifyAssemblyContainsReleaseWebResource(Assembly assembly, string releaseResourceName)
        {
            if (!AssemblyContainsWebResource(assembly, releaseResourceName))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, AtlasWeb.WebResourceUtil_AssemblyDoesNotContainReleaseWebResource, new object[] { assembly, releaseResourceName }));
            }
        }
    }
}

