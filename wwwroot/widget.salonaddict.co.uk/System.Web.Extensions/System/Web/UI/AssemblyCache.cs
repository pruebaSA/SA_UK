namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal static class AssemblyCache
    {
        private static readonly Hashtable _assemblyCache = Hashtable.Synchronized(new Hashtable());
        public static readonly Assembly SystemWebExtensions = typeof(AssemblyRef).Assembly;

        public static Assembly Load(string assemblyName)
        {
            Assembly assembly = (Assembly) _assemblyCache[assemblyName];
            if (assembly == null)
            {
                assembly = Assembly.Load(assemblyName);
                _assemblyCache[assemblyName] = assembly;
            }
            return assembly;
        }
    }
}

