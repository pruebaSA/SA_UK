namespace System.Security
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, Flags, ComVisible(true)]
    public enum HostSecurityManagerOptions
    {
        AllFlags = 0x1f,
        HostAppDomainEvidence = 1,
        HostAssemblyEvidence = 4,
        HostDetermineApplicationTrust = 8,
        HostPolicyLevel = 2,
        HostResolvePolicy = 0x10,
        None = 0
    }
}

