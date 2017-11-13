namespace System.Net.NetworkInformation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct FixedInfo
    {
        internal FIXED_INFO info;
        internal IPAddressCollection dnsAddresses;
        internal FixedInfo(FIXED_INFO info)
        {
            this.info = info;
            this.dnsAddresses = info.DnsServerList.ToIPAddressCollection();
        }

        internal IPAddressCollection DnsAddresses =>
            this.dnsAddresses;
        internal string HostName =>
            this.info.hostName;
        internal string DomainName =>
            this.info.domainName;
        internal NetBiosNodeType NodeType =>
            this.info.nodeType;
        internal string ScopeId =>
            this.info.scopeId;
        internal bool EnableRouting =>
            this.info.enableRouting;
        internal bool EnableProxy =>
            this.info.enableProxy;
        internal bool EnableDns =>
            this.info.enableDns;
    }
}

