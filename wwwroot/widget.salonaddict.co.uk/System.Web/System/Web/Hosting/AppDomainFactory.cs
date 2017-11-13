namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AppDomainFactory : IAppDomainFactory
    {
        private AppManagerAppDomainFactory _realFactory = new AppManagerAppDomainFactory();

        [return: MarshalAs(UnmanagedType.Interface)]
        public object Create(string module, string typeName, string appId, string appPath, string strUrlOfAppOrigin, int iZone) => 
            this._realFactory.Create(appId, appPath);
    }
}

