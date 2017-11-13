namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Medium), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.High)]
    public abstract class VirtualPathProvider : MarshalByRefObject
    {
        private VirtualPathProvider _previous;

        protected VirtualPathProvider()
        {
        }

        public virtual string CombineVirtualPaths(string basePath, string relativePath)
        {
            string basepath = null;
            if (!string.IsNullOrEmpty(basePath))
            {
                basepath = UrlPath.GetDirectory(basePath);
            }
            return UrlPath.Combine(basepath, relativePath);
        }

        internal VirtualPath CombineVirtualPaths(VirtualPath basePath, VirtualPath relativePath) => 
            VirtualPath.Create(this.CombineVirtualPaths(basePath.VirtualPathString, relativePath.VirtualPathString));

        internal static VirtualPath CombineVirtualPathsInternal(VirtualPath basePath, VirtualPath relativePath)
        {
            VirtualPathProvider virtualPathProvider = HostingEnvironment.VirtualPathProvider;
            if (virtualPathProvider != null)
            {
                return virtualPathProvider.CombineVirtualPaths(basePath, relativePath);
            }
            return basePath.Parent.Combine(relativePath);
        }

        public virtual bool DirectoryExists(string virtualDir) => 
            this._previous?.DirectoryExists(virtualDir);

        internal bool DirectoryExists(VirtualPath virtualDir) => 
            this.DirectoryExists(virtualDir.VirtualPathString);

        internal static bool DirectoryExistsNoThrow(string virtualDir)
        {
            try
            {
                return HostingEnvironment.VirtualPathProvider.DirectoryExists(virtualDir);
            }
            catch
            {
                return false;
            }
        }

        internal static bool DirectoryExistsNoThrow(VirtualPath virtualDir) => 
            DirectoryExistsNoThrow(virtualDir.VirtualPathString);

        public virtual bool FileExists(string virtualPath) => 
            this._previous?.FileExists(virtualPath);

        internal bool FileExists(VirtualPath virtualPath) => 
            this.FileExists(virtualPath.VirtualPathString);

        internal static CacheDependency GetCacheDependency(VirtualPath virtualPath) => 
            HostingEnvironment.VirtualPathProvider.GetCacheDependency(virtualPath, new SingleObjectCollection(virtualPath.VirtualPathString), DateTime.MaxValue);

        public virtual CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) => 
            this._previous?.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);

        internal CacheDependency GetCacheDependency(VirtualPath virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) => 
            this.GetCacheDependency(virtualPath.VirtualPathString, virtualPathDependencies, utcStart);

        public virtual string GetCacheKey(string virtualPath) => 
            null;

        internal string GetCacheKey(VirtualPath virtualPath) => 
            this.GetCacheKey(virtualPath.VirtualPathString);

        public virtual VirtualDirectory GetDirectory(string virtualDir) => 
            this._previous?.GetDirectory(virtualDir);

        internal VirtualDirectory GetDirectory(VirtualPath virtualDir) => 
            this.GetDirectoryWithCheck(virtualDir.VirtualPathString);

        internal VirtualDirectory GetDirectoryWithCheck(string virtualPath)
        {
            VirtualDirectory directory = this.GetDirectory(virtualPath);
            if (directory == null)
            {
                return null;
            }
            if (!StringUtil.EqualsIgnoreCase(virtualPath, directory.VirtualPath))
            {
                throw new HttpException(System.Web.SR.GetString("Bad_VirtualPath_in_VirtualFileBase", new object[] { "VirtualDirectory", directory.VirtualPath, virtualPath }));
            }
            return directory;
        }

        public virtual VirtualFile GetFile(string virtualPath) => 
            this._previous?.GetFile(virtualPath);

        internal VirtualFile GetFile(VirtualPath virtualPath) => 
            this.GetFileWithCheck(virtualPath.VirtualPathString);

        public virtual string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies) => 
            this._previous?.GetFileHash(virtualPath, virtualPathDependencies);

        internal string GetFileHash(VirtualPath virtualPath, IEnumerable virtualPathDependencies) => 
            this.GetFileHash(virtualPath.VirtualPathString, virtualPathDependencies);

        internal VirtualFile GetFileWithCheck(string virtualPath)
        {
            VirtualFile file = this.GetFile(virtualPath);
            if (file == null)
            {
                return null;
            }
            if (!StringUtil.EqualsIgnoreCase(virtualPath, file.VirtualPath))
            {
                throw new HttpException(System.Web.SR.GetString("Bad_VirtualPath_in_VirtualFileBase", new object[] { "VirtualFile", file.VirtualPath, virtualPath }));
            }
            return file;
        }

        protected virtual void Initialize()
        {
        }

        internal virtual void Initialize(VirtualPathProvider previous)
        {
            this._previous = previous;
            this.Initialize();
        }

        public override object InitializeLifetimeService() => 
            null;

        public static Stream OpenFile(string virtualPath) => 
            HostingEnvironment.VirtualPathProvider.GetFileWithCheck(virtualPath).Open();

        internal static Stream OpenFile(VirtualPath virtualPath) => 
            OpenFile(virtualPath.VirtualPathString);

        protected internal VirtualPathProvider Previous =>
            this._previous;
    }
}

