namespace System.Web
{
    using System;
    using System.Security.Permissions;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public static class VirtualPathUtility
    {
        public static string AppendTrailingSlash(string virtualPath) => 
            UrlPath.AppendSlashToPathIfNeeded(virtualPath);

        public static string Combine(string basePath, string relativePath) => 
            VirtualPath.Combine(VirtualPath.CreateNonRelative(basePath), VirtualPath.Create(relativePath)).VirtualPathStringWhicheverAvailable;

        public static string GetDirectory(string virtualPath)
        {
            VirtualPath parent = VirtualPath.CreateNonRelative(virtualPath).Parent;
            return parent?.VirtualPathStringWhicheverAvailable;
        }

        public static string GetExtension(string virtualPath) => 
            VirtualPath.Create(virtualPath).Extension;

        public static string GetFileName(string virtualPath) => 
            VirtualPath.CreateNonRelative(virtualPath).FileName;

        public static bool IsAbsolute(string virtualPath)
        {
            VirtualPath path = VirtualPath.Create(virtualPath);
            return (!path.IsRelative && (path.VirtualPathStringIfAvailable != null));
        }

        public static bool IsAppRelative(string virtualPath) => 
            (VirtualPath.Create(virtualPath).VirtualPathStringIfAvailable == null);

        public static string MakeRelative(string fromPath, string toPath) => 
            UrlPath.MakeRelative(fromPath, toPath);

        public static string RemoveTrailingSlash(string virtualPath) => 
            UrlPath.RemoveSlashFromPathIfNeeded(virtualPath);

        public static string ToAbsolute(string virtualPath) => 
            VirtualPath.CreateNonRelative(virtualPath).VirtualPathString;

        public static string ToAbsolute(string virtualPath, string applicationPath)
        {
            VirtualPath path = VirtualPath.CreateNonRelative(virtualPath);
            if (path.VirtualPathStringIfAvailable != null)
            {
                return path.VirtualPathStringIfAvailable;
            }
            VirtualPath path2 = VirtualPath.CreateAbsoluteTrailingSlash(applicationPath);
            return UrlPath.MakeVirtualPathAppAbsolute(path.AppRelativeVirtualPathString, path2.VirtualPathString);
        }

        public static string ToAppRelative(string virtualPath) => 
            VirtualPath.CreateNonRelative(virtualPath).AppRelativeVirtualPathString;

        public static string ToAppRelative(string virtualPath, string applicationPath)
        {
            VirtualPath path = VirtualPath.CreateNonRelative(virtualPath);
            if (path.AppRelativeVirtualPathStringIfAvailable != null)
            {
                return path.AppRelativeVirtualPathStringIfAvailable;
            }
            VirtualPath path2 = VirtualPath.CreateAbsoluteTrailingSlash(applicationPath);
            return UrlPath.MakeVirtualPathAppRelative(path.VirtualPathString, path2.VirtualPathString, true);
        }
    }
}

