namespace System.Web
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Permissions;
    using System.Web.Caching;
    using System.Web.Hosting;
    using System.Web.Util;

    [Serializable]
    internal sealed class VirtualPath : IComparable
    {
        private string _appRelativeVirtualPath;
        private string _virtualPath;
        private const int appRelativeAttempted = 4;
        private SimpleBitVector32 flags;
        private const int isWithinAppRoot = 2;
        private const int isWithinAppRootComputed = 1;
        internal static VirtualPath RootVirtualPath = Create("/");
        private static char[] s_illegalVirtualPathChars = new char[] { ':', '?', '*', '\0' };
        private static char[] s_illegalVirtualPathChars_VerCompat = new char[1];
        private static object s_VerCompatLock = new object();
        private static bool s_VerCompatRegLookedUp = false;

        private VirtualPath()
        {
        }

        private VirtualPath(string virtualPath)
        {
            if (UrlPath.IsAppRelativePath(virtualPath))
            {
                this._appRelativeVirtualPath = virtualPath;
            }
            else
            {
                this._virtualPath = virtualPath;
            }
        }

        public VirtualPath Combine(VirtualPath relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException("relativePath");
            }
            if (!relativePath.IsRelative)
            {
                return relativePath;
            }
            this.FailIfRelativePath();
            return new VirtualPath(UrlPath.Combine(this.VirtualPathStringWhicheverAvailable, relativePath.VirtualPathString));
        }

        internal static VirtualPath Combine(VirtualPath v1, VirtualPath v2) => 
            v1?.Combine(v2);

        public VirtualPath CombineWithAppRoot() => 
            HttpRuntime.AppDomainAppVirtualPathObject.Combine(this);

        private static bool ContainsIllegalVirtualPathChars(string virtualPath)
        {
            if (!s_VerCompatRegLookedUp)
            {
                LookUpRegForVerCompat();
            }
            return (virtualPath.IndexOfAny(s_illegalVirtualPathChars) >= 0);
        }

        private void CopyFlagsFrom(VirtualPath virtualPath, int mask)
        {
            this.flags.IntegerValue |= virtualPath.flags.IntegerValue & mask;
        }

        public static VirtualPath Create(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAllPath);

        public static VirtualPath Create(string virtualPath, VirtualPathOptions options)
        {
            if (virtualPath != null)
            {
                virtualPath = virtualPath.Trim();
            }
            if (string.IsNullOrEmpty(virtualPath))
            {
                if ((options & VirtualPathOptions.AllowNull) == 0)
                {
                    throw new ArgumentNullException("virtualPath");
                }
                return null;
            }
            if (ContainsIllegalVirtualPathChars(virtualPath))
            {
                throw new HttpException(System.Web.SR.GetString("Invalid_vpath", new object[] { virtualPath }));
            }
            string objB = UrlPath.FixVirtualPathSlashes(virtualPath);
            if (((options & VirtualPathOptions.FailIfMalformed) != 0) && !object.ReferenceEquals(virtualPath, objB))
            {
                throw new HttpException(System.Web.SR.GetString("Invalid_vpath", new object[] { virtualPath }));
            }
            virtualPath = objB;
            if ((options & VirtualPathOptions.EnsureTrailingSlash) != 0)
            {
                virtualPath = UrlPath.AppendSlashToPathIfNeeded(virtualPath);
            }
            VirtualPath path = new VirtualPath();
            if (UrlPath.IsAppRelativePath(virtualPath))
            {
                virtualPath = UrlPath.ReduceVirtualPath(virtualPath);
                if (virtualPath[0] == '~')
                {
                    if ((options & VirtualPathOptions.AllowAppRelativePath) == 0)
                    {
                        throw new ArgumentException(System.Web.SR.GetString("VirtualPath_AllowAppRelativePath", new object[] { virtualPath }));
                    }
                    path._appRelativeVirtualPath = virtualPath;
                    return path;
                }
                if ((options & VirtualPathOptions.AllowAbsolutePath) == 0)
                {
                    throw new ArgumentException(System.Web.SR.GetString("VirtualPath_AllowAbsolutePath", new object[] { virtualPath }));
                }
                path._virtualPath = virtualPath;
                return path;
            }
            if (virtualPath[0] != '/')
            {
                if ((options & VirtualPathOptions.AllowRelativePath) == 0)
                {
                    throw new ArgumentException(System.Web.SR.GetString("VirtualPath_AllowRelativePath", new object[] { virtualPath }));
                }
                path._virtualPath = virtualPath;
                return path;
            }
            if ((options & VirtualPathOptions.AllowAbsolutePath) == 0)
            {
                throw new ArgumentException(System.Web.SR.GetString("VirtualPath_AllowAbsolutePath", new object[] { virtualPath }));
            }
            path._virtualPath = UrlPath.ReduceVirtualPath(virtualPath);
            return path;
        }

        public static VirtualPath CreateAbsolute(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAbsolutePath);

        public static VirtualPath CreateAbsoluteAllowNull(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.AllowNull);

        public static VirtualPath CreateAbsoluteTrailingSlash(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash);

        public static VirtualPath CreateAllowNull(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAllPath | VirtualPathOptions.AllowNull);

        public static VirtualPath CreateNonRelative(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath);

        public static VirtualPath CreateNonRelativeAllowNull(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.AllowNull);

        public static VirtualPath CreateNonRelativeTrailingSlash(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash);

        public static VirtualPath CreateNonRelativeTrailingSlashAllowNull(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAppRelativePath | VirtualPathOptions.AllowAbsolutePath | VirtualPathOptions.EnsureTrailingSlash | VirtualPathOptions.AllowNull);

        public static VirtualPath CreateTrailingSlash(string virtualPath) => 
            Create(virtualPath, VirtualPathOptions.AllowAllPath | VirtualPathOptions.EnsureTrailingSlash);

        public bool DirectoryExists() => 
            HostingEnvironment.VirtualPathProvider.DirectoryExists(this);

        public override bool Equals(object value)
        {
            if (value == null)
            {
                return false;
            }
            VirtualPath path = value as VirtualPath;
            if (path == null)
            {
                return false;
            }
            return EqualsHelper(path, this);
        }

        public static bool Equals(VirtualPath v1, VirtualPath v2) => 
            ((v1 == v2) || (((v1 != null) && (v2 != null)) && EqualsHelper(v1, v2)));

        private static bool EqualsHelper(VirtualPath v1, VirtualPath v2) => 
            (StringComparer.InvariantCultureIgnoreCase.Compare(v1.VirtualPathString, v2.VirtualPathString) == 0);

        internal void FailIfNotWithinAppRoot()
        {
            if (!this.IsWithinAppRoot)
            {
                throw new ArgumentException(System.Web.SR.GetString("Cross_app_not_allowed", new object[] { this.VirtualPathString }));
            }
        }

        internal void FailIfRelativePath()
        {
            if (this.IsRelative)
            {
                throw new ArgumentException(System.Web.SR.GetString("VirtualPath_AllowRelativePath", new object[] { this._virtualPath }));
            }
        }

        public bool FileExists() => 
            HostingEnvironment.VirtualPathProvider.FileExists(this);

        internal static string GetAppRelativeVirtualPathString(VirtualPath virtualPath)
        {
            if (virtualPath != null)
            {
                return virtualPath.AppRelativeVirtualPathString;
            }
            return null;
        }

        internal static string GetAppRelativeVirtualPathStringOrEmpty(VirtualPath virtualPath)
        {
            if (virtualPath != null)
            {
                return virtualPath.AppRelativeVirtualPathString;
            }
            return string.Empty;
        }

        public CacheDependency GetCacheDependency(IEnumerable virtualPathDependencies, DateTime utcStart) => 
            HostingEnvironment.VirtualPathProvider.GetCacheDependency(this, virtualPathDependencies, utcStart);

        public string GetCacheKey() => 
            HostingEnvironment.VirtualPathProvider.GetCacheKey(this);

        public VirtualDirectory GetDirectory() => 
            HostingEnvironment.VirtualPathProvider.GetDirectory(this);

        public VirtualFile GetFile() => 
            HostingEnvironment.VirtualPathProvider.GetFile(this);

        public string GetFileHash(IEnumerable virtualPathDependencies) => 
            HostingEnvironment.VirtualPathProvider.GetFileHash(this, virtualPathDependencies);

        public override int GetHashCode() => 
            StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.VirtualPathString);

        internal static string GetVirtualPathString(VirtualPath virtualPath)
        {
            if (virtualPath != null)
            {
                return virtualPath.VirtualPathString;
            }
            return null;
        }

        internal static string GetVirtualPathStringNoTrailingSlash(VirtualPath virtualPath)
        {
            if (virtualPath != null)
            {
                return virtualPath.VirtualPathStringNoTrailingSlash;
            }
            return null;
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static void LookUpRegForVerCompat()
        {
            lock (s_VerCompatLock)
            {
                if (!s_VerCompatRegLookedUp)
                {
                    try
                    {
                        object obj2 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\ASP.NET", "VerificationCompatibility", 0);
                        if (((obj2 != null) && ((obj2 is int) || (obj2 is uint))) && (((int) obj2) == 1))
                        {
                            s_illegalVirtualPathChars = s_illegalVirtualPathChars_VerCompat;
                        }
                        s_VerCompatRegLookedUp = true;
                    }
                    catch
                    {
                    }
                }
            }
        }

        public VirtualPath MakeRelative(VirtualPath toVirtualPath)
        {
            VirtualPath path = new VirtualPath();
            this.FailIfRelativePath();
            toVirtualPath.FailIfRelativePath();
            path._virtualPath = UrlPath.MakeRelative(this.VirtualPathString, toVirtualPath.VirtualPathString);
            return path;
        }

        public string MapPath() => 
            HostingEnvironment.MapPath(this);

        internal string MapPathInternal() => 
            HostingEnvironment.MapPathInternal(this);

        internal string MapPathInternal(bool permitNull) => 
            HostingEnvironment.MapPathInternal(this, permitNull);

        internal string MapPathInternal(VirtualPath baseVirtualDir, bool allowCrossAppMapping) => 
            HostingEnvironment.MapPathInternal(this, baseVirtualDir, allowCrossAppMapping);

        public static bool operator ==(VirtualPath v1, VirtualPath v2) => 
            Equals(v1, v2);

        public static bool operator !=(VirtualPath v1, VirtualPath v2) => 
            !Equals(v1, v2);

        public Stream OpenFile() => 
            VirtualPathProvider.OpenFile(this);

        internal VirtualPath SimpleCombine(string relativePath) => 
            this.SimpleCombine(relativePath, false);

        private VirtualPath SimpleCombine(string filename, bool addTrailingSlash)
        {
            string virtualPath = this.VirtualPathStringWhicheverAvailable + filename;
            if (addTrailingSlash)
            {
                virtualPath = virtualPath + "/";
            }
            VirtualPath path = new VirtualPath(virtualPath);
            path.CopyFlagsFrom(this, 7);
            return path;
        }

        internal VirtualPath SimpleCombineWithDir(string directoryName) => 
            this.SimpleCombine(directoryName, true);

        int IComparable.CompareTo(object obj)
        {
            VirtualPath path = obj as VirtualPath;
            if (path == null)
            {
                throw new ArgumentException();
            }
            if (path == this)
            {
                return 0;
            }
            return StringComparer.InvariantCultureIgnoreCase.Compare(this.VirtualPathString, path.VirtualPathString);
        }

        public override string ToString()
        {
            if ((this._virtualPath == null) && (HttpRuntime.AppDomainAppVirtualPathObject == null))
            {
                return this._appRelativeVirtualPath;
            }
            return this.VirtualPathString;
        }

        [Conditional("DBG")]
        private void ValidateState()
        {
        }

        public string AppRelativeVirtualPathString
        {
            get
            {
                string appRelativeVirtualPathStringOrNull = this.AppRelativeVirtualPathStringOrNull;
                if (appRelativeVirtualPathStringOrNull == null)
                {
                    return this._virtualPath;
                }
                return appRelativeVirtualPathStringOrNull;
            }
        }

        internal string AppRelativeVirtualPathStringIfAvailable =>
            this._appRelativeVirtualPath;

        internal string AppRelativeVirtualPathStringOrNull
        {
            get
            {
                if (this._appRelativeVirtualPath == null)
                {
                    if (this.flags[4])
                    {
                        return null;
                    }
                    if (HttpRuntime.AppDomainAppVirtualPathObject == null)
                    {
                        throw new HttpException(System.Web.SR.GetString("VirtualPath_CantMakeAppRelative", new object[] { this._virtualPath }));
                    }
                    this._appRelativeVirtualPath = UrlPath.MakeVirtualPathAppRelativeOrNull(this._virtualPath);
                    this.flags[4] = true;
                    if (this._appRelativeVirtualPath == null)
                    {
                        return null;
                    }
                }
                return this._appRelativeVirtualPath;
            }
        }

        public string Extension =>
            UrlPath.GetExtension(this.VirtualPathString);

        public string FileName =>
            UrlPath.GetFileName(this.VirtualPathStringNoTrailingSlash);

        internal bool HasTrailingSlash
        {
            get
            {
                if (this._virtualPath != null)
                {
                    return UrlPath.HasTrailingSlash(this._virtualPath);
                }
                return UrlPath.HasTrailingSlash(this._appRelativeVirtualPath);
            }
        }

        public bool IsRelative =>
            ((this._virtualPath != null) && (this._virtualPath[0] != '/'));

        public bool IsRoot =>
            (this._virtualPath == "/");

        public bool IsWithinAppRoot
        {
            get
            {
                if (!this.flags[1])
                {
                    if (HttpRuntime.AppDomainIdInternal == null)
                    {
                        return true;
                    }
                    if (this.flags[4])
                    {
                        this.flags[2] = this._appRelativeVirtualPath != null;
                    }
                    else
                    {
                        this.flags[2] = UrlPath.IsEqualOrSubpath(HttpRuntime.AppDomainAppVirtualPathString, this.VirtualPathString);
                    }
                    this.flags[1] = true;
                }
                return this.flags[2];
            }
        }

        public VirtualPath Parent
        {
            get
            {
                this.FailIfRelativePath();
                if (this.IsRoot)
                {
                    return null;
                }
                string virtualPathStringNoTrailingSlash = UrlPath.RemoveSlashFromPathIfNeeded(this.VirtualPathStringWhicheverAvailable);
                if (virtualPathStringNoTrailingSlash == "~")
                {
                    virtualPathStringNoTrailingSlash = this.VirtualPathStringNoTrailingSlash;
                }
                int num = virtualPathStringNoTrailingSlash.LastIndexOf('/');
                if (num == 0)
                {
                    return RootVirtualPath;
                }
                return new VirtualPath(virtualPathStringNoTrailingSlash.Substring(0, num + 1));
            }
        }

        public string VirtualPathString
        {
            get
            {
                if (this._virtualPath == null)
                {
                    if (HttpRuntime.AppDomainAppVirtualPathObject == null)
                    {
                        throw new HttpException(System.Web.SR.GetString("VirtualPath_CantMakeAppAbsolute", new object[] { this._appRelativeVirtualPath }));
                    }
                    if (this._appRelativeVirtualPath.Length == 1)
                    {
                        this._virtualPath = HttpRuntime.AppDomainAppVirtualPath;
                    }
                    else
                    {
                        this._virtualPath = HttpRuntime.AppDomainAppVirtualPathString + this._appRelativeVirtualPath.Substring(2);
                    }
                }
                return this._virtualPath;
            }
        }

        internal string VirtualPathStringIfAvailable =>
            this._virtualPath;

        internal string VirtualPathStringNoTrailingSlash =>
            UrlPath.RemoveSlashFromPathIfNeeded(this.VirtualPathString);

        internal string VirtualPathStringWhicheverAvailable
        {
            get
            {
                if (this._virtualPath == null)
                {
                    return this._appRelativeVirtualPath;
                }
                return this._virtualPath;
            }
        }
    }
}

