namespace System.Web.Configuration
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpConfigurationContext
    {
        private string vpath;

        internal HttpConfigurationContext(string vpath)
        {
            this.vpath = vpath;
        }

        public string VirtualPath =>
            this.vpath;
    }
}

