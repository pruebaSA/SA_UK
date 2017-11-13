namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Threading;

    internal sealed class SettingsSectionInternal
    {
        private bool alwaysUseCompletionPortsForAccept;
        private bool alwaysUseCompletionPortsForConnect;
        private bool checkCertificateName;
        private bool checkCertificateRevocationList;
        private int dnsRefreshTimeout;
        private int downloadTimeout;
        private bool enableDnsRoundRobin;
        private bool expect100Continue;
        private bool ipv6Enabled;
        private int maximumErrorResponseLength;
        private int maximumResponseHeadersLength;
        private int maximumUnauthorizedUploadLength;
        private bool performanceCountersEnabled;
        private static object s_InternalSyncObject;
        private static SettingsSectionInternal s_settings;
        private bool useNagleAlgorithm;
        private bool useUnsafeHeaderParsing;

        internal SettingsSectionInternal(SettingsSection section)
        {
            if (section == null)
            {
                section = new SettingsSection();
            }
            this.alwaysUseCompletionPortsForConnect = section.Socket.AlwaysUseCompletionPortsForConnect;
            this.alwaysUseCompletionPortsForAccept = section.Socket.AlwaysUseCompletionPortsForAccept;
            this.checkCertificateName = section.ServicePointManager.CheckCertificateName;
            this.CheckCertificateRevocationList = section.ServicePointManager.CheckCertificateRevocationList;
            this.DnsRefreshTimeout = section.ServicePointManager.DnsRefreshTimeout;
            this.ipv6Enabled = section.Ipv6.Enabled;
            this.EnableDnsRoundRobin = section.ServicePointManager.EnableDnsRoundRobin;
            this.Expect100Continue = section.ServicePointManager.Expect100Continue;
            this.maximumUnauthorizedUploadLength = section.HttpWebRequest.MaximumUnauthorizedUploadLength;
            this.maximumResponseHeadersLength = section.HttpWebRequest.MaximumResponseHeadersLength;
            this.maximumErrorResponseLength = section.HttpWebRequest.MaximumErrorResponseLength;
            this.useUnsafeHeaderParsing = section.HttpWebRequest.UseUnsafeHeaderParsing;
            this.UseNagleAlgorithm = section.ServicePointManager.UseNagleAlgorithm;
            TimeSpan downloadTimeout = section.WebProxyScript.DownloadTimeout;
            this.downloadTimeout = ((downloadTimeout == TimeSpan.MaxValue) || (downloadTimeout == TimeSpan.Zero)) ? -1 : ((int) downloadTimeout.TotalMilliseconds);
            this.performanceCountersEnabled = section.PerformanceCounters.Enabled;
            NetworkingPerfCounters.Initialize();
        }

        internal static SettingsSectionInternal GetSection() => 
            new SettingsSectionInternal((SettingsSection) System.Configuration.PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SettingsSectionPath));

        internal bool AlwaysUseCompletionPortsForAccept =>
            this.alwaysUseCompletionPortsForAccept;

        internal bool AlwaysUseCompletionPortsForConnect =>
            this.alwaysUseCompletionPortsForConnect;

        internal bool CheckCertificateName =>
            this.checkCertificateName;

        internal bool CheckCertificateRevocationList
        {
            get => 
                this.checkCertificateRevocationList;
            set
            {
                this.checkCertificateRevocationList = value;
            }
        }

        internal int DnsRefreshTimeout
        {
            get => 
                this.dnsRefreshTimeout;
            set
            {
                this.dnsRefreshTimeout = value;
            }
        }

        internal int DownloadTimeout =>
            this.downloadTimeout;

        internal bool EnableDnsRoundRobin
        {
            get => 
                this.enableDnsRoundRobin;
            set
            {
                this.enableDnsRoundRobin = value;
            }
        }

        internal bool Expect100Continue
        {
            get => 
                this.expect100Continue;
            set
            {
                this.expect100Continue = value;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        internal bool Ipv6Enabled =>
            this.ipv6Enabled;

        internal int MaximumErrorResponseLength
        {
            get => 
                this.maximumErrorResponseLength;
            set
            {
                this.maximumErrorResponseLength = value;
            }
        }

        internal int MaximumResponseHeadersLength
        {
            get => 
                this.maximumResponseHeadersLength;
            set
            {
                this.maximumResponseHeadersLength = value;
            }
        }

        internal int MaximumUnauthorizedUploadLength =>
            this.maximumUnauthorizedUploadLength;

        internal bool PerformanceCountersEnabled =>
            this.performanceCountersEnabled;

        internal static SettingsSectionInternal Section
        {
            get
            {
                if (s_settings == null)
                {
                    lock (InternalSyncObject)
                    {
                        if (s_settings == null)
                        {
                            s_settings = new SettingsSectionInternal((SettingsSection) System.Configuration.PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SettingsSectionPath));
                        }
                    }
                }
                return s_settings;
            }
        }

        internal bool UseNagleAlgorithm
        {
            get => 
                this.useNagleAlgorithm;
            set
            {
                this.useNagleAlgorithm = value;
            }
        }

        internal bool UseUnsafeHeaderParsing =>
            this.useUnsafeHeaderParsing;
    }
}

