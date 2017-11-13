namespace System.DirectoryServices.ActiveDirectory
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public class ForestTrustDomainInformation
    {
        private string dnsName;
        private string nbName;
        private string sid;
        private ForestTrustDomainStatus status;
        internal LARGE_INTEGER time;

        internal ForestTrustDomainInformation(int flag, LSA_FOREST_TRUST_DOMAIN_INFO domainInfo, LARGE_INTEGER time)
        {
            this.status = (ForestTrustDomainStatus) flag;
            this.dnsName = Marshal.PtrToStringUni(domainInfo.DNSNameBuffer, domainInfo.DNSNameLength / 2);
            this.nbName = Marshal.PtrToStringUni(domainInfo.NetBIOSNameBuffer, domainInfo.NetBIOSNameLength / 2);
            IntPtr zero = IntPtr.Zero;
            if (UnsafeNativeMethods.ConvertSidToStringSidW(domainInfo.sid, ref zero) == 0)
            {
                throw ExceptionHelper.GetExceptionFromErrorCode(Marshal.GetLastWin32Error());
            }
            try
            {
                this.sid = Marshal.PtrToStringUni(zero);
            }
            finally
            {
                UnsafeNativeMethods.LocalFree(zero);
            }
            this.time = time;
        }

        public string DnsName =>
            this.dnsName;

        public string DomainSid =>
            this.sid;

        public string NetBiosName =>
            this.nbName;

        public ForestTrustDomainStatus Status
        {
            get => 
                this.status;
            set
            {
                if ((((value != ForestTrustDomainStatus.Enabled) && (value != ForestTrustDomainStatus.SidAdminDisabled)) && ((value != ForestTrustDomainStatus.SidConflictDisabled) && (value != ForestTrustDomainStatus.NetBiosNameAdminDisabled))) && (value != ForestTrustDomainStatus.NetBiosNameConflictDisabled))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(ForestTrustDomainStatus));
                }
                this.status = value;
            }
        }
    }
}

