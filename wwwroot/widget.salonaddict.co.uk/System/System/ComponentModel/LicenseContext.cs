namespace System.ComponentModel
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class LicenseContext : IServiceProvider
    {
        public virtual string GetSavedLicenseKey(Type type, Assembly resourceAssembly) => 
            null;

        public virtual object GetService(Type type) => 
            null;

        public virtual void SetSavedLicenseKey(Type type, string key)
        {
        }

        public virtual LicenseUsageMode UsageMode =>
            LicenseUsageMode.Runtime;
    }
}

