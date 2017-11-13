namespace System.Web.Compilation
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;

    internal abstract class BuildResultCache
    {
        protected BuildResultCache()
        {
        }

        internal void CacheBuildResult(string cacheKey, BuildResult result, DateTime utcStart)
        {
            this.CacheBuildResult(cacheKey, result, 0L, utcStart);
        }

        internal abstract void CacheBuildResult(string cacheKey, BuildResult result, long hashCode, DateTime utcStart);
        internal static string GetAssemblyCacheKey(Assembly assembly) => 
            GetAssemblyCacheKeyFromName(assembly.GetName().Name);

        internal static string GetAssemblyCacheKey(string assemblyPath) => 
            GetAssemblyCacheKeyFromName(Util.GetAssemblyNameFromFileName(Path.GetFileName(assemblyPath)));

        internal static string GetAssemblyCacheKeyFromName(string assemblyName) => 
            ("y" + assemblyName.ToLowerInvariant());

        internal BuildResult GetBuildResult(string cacheKey) => 
            this.GetBuildResult(cacheKey, null, 0L);

        internal abstract BuildResult GetBuildResult(string cacheKey, VirtualPath virtualPath, long hashCode);
    }
}

