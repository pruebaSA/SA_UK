namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web.Caching;
    using System.Web.Util;

    internal class MapPathBasedVirtualPathProvider : VirtualPathProvider
    {
        public override bool DirectoryExists(string virtualDir) => 
            Directory.Exists(HostingEnvironment.MapPathInternal(virtualDir));

        public override bool FileExists(string virtualPath) => 
            File.Exists(HostingEnvironment.MapPathInternal(virtualPath));

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPathDependencies == null)
            {
                return null;
            }
            StringCollection strings = null;
            foreach (string str in virtualPathDependencies)
            {
                string str2 = HostingEnvironment.MapPathInternal(str);
                if (strings == null)
                {
                    strings = new StringCollection();
                }
                strings.Add(str2);
            }
            if (strings == null)
            {
                return null;
            }
            string[] array = new string[strings.Count];
            strings.CopyTo(array, 0);
            return new CacheDependency(0, array, utcStart);
        }

        public override VirtualDirectory GetDirectory(string virtualDir) => 
            new MapPathBasedVirtualDirectory(virtualDir);

        public override VirtualFile GetFile(string virtualPath) => 
            new MapPathBasedVirtualFile(virtualPath);

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            foreach (string str in virtualPathDependencies)
            {
                string fileName = HostingEnvironment.MapPathInternal(str);
                combiner.AddFile(fileName);
            }
            return combiner.CombinedHashString;
        }
    }
}

