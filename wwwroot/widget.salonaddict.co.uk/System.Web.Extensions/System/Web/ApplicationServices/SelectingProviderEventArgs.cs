namespace System.Web.ApplicationServices
{
    using System;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SelectingProviderEventArgs : EventArgs
    {
        private string _providerName;
        private IPrincipal _user;

        private SelectingProviderEventArgs()
        {
        }

        internal SelectingProviderEventArgs(IPrincipal user, string providerName)
        {
            this._user = user;
            this._providerName = providerName;
        }

        public string ProviderName
        {
            get => 
                this._providerName;
            set
            {
                this._providerName = value;
            }
        }

        public IPrincipal User =>
            this._user;
    }
}

