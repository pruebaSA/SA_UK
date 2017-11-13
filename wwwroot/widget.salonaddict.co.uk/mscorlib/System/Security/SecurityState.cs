namespace System.Security
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true), PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
    public abstract class SecurityState
    {
        protected SecurityState()
        {
        }

        public abstract void EnsureState();
        public bool IsStateAvailable()
        {
            AppDomainManager currentAppDomainManager = AppDomainManager.CurrentAppDomainManager;
            return currentAppDomainManager?.CheckSecuritySettings(this);
        }
    }
}

