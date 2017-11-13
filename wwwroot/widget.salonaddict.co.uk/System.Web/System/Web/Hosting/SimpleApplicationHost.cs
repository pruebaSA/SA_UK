namespace System.Web.Hosting
{
    using System;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Util;

    internal class SimpleApplicationHost : MarshalByRefObject, IApplicationHost
    {
        private string _appPhysicalPath;
        private VirtualPath _appVirtualPath;

        internal SimpleApplicationHost(VirtualPath virtualPath, string physicalPath)
        {
            if (string.IsNullOrEmpty(physicalPath))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("physicalPath");
            }
            if (FileUtil.IsSuspiciousPhysicalPath(physicalPath))
            {
                throw ExceptionUtil.ParameterInvalid(physicalPath);
            }
            this._appVirtualPath = virtualPath;
            this._appPhysicalPath = StringUtil.StringEndsWith(physicalPath, @"\") ? physicalPath : (physicalPath + @"\");
        }

        public string GetVirtualPath() => 
            this._appVirtualPath.VirtualPathString;

        public override object InitializeLifetimeService() => 
            null;

        public void MessageReceived()
        {
        }

        IConfigMapPathFactory IApplicationHost.GetConfigMapPathFactory() => 
            new SimpleConfigMapPathFactory();

        IntPtr IApplicationHost.GetConfigToken() => 
            IntPtr.Zero;

        string IApplicationHost.GetPhysicalPath() => 
            this._appPhysicalPath;

        string IApplicationHost.GetSiteID() => 
            "1";

        string IApplicationHost.GetSiteName() => 
            WebConfigurationHost.DefaultSiteName;
    }
}

