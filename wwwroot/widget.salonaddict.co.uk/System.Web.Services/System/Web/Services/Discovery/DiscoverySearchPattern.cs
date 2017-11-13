namespace System.Web.Services.Discovery
{
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class DiscoverySearchPattern
    {
        protected DiscoverySearchPattern()
        {
        }

        public abstract DiscoveryReference GetDiscoveryReference(string filename);

        public abstract string Pattern { get; }
    }
}

