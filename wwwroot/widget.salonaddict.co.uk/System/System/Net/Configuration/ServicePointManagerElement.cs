﻿namespace System.Net.Configuration
{
    using System;
    using System.Configuration;
    using System.Net;

    public sealed class ServicePointManagerElement : ConfigurationElement
    {
        private readonly ConfigurationProperty checkCertificateName = new ConfigurationProperty("checkCertificateName", typeof(bool), true, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty checkCertificateRevocationList = new ConfigurationProperty("checkCertificateRevocationList", typeof(bool), false, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty dnsRefreshTimeout = new ConfigurationProperty("dnsRefreshTimeout", typeof(int), 0x1d4c0, null, new TimeoutValidator(true), ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty enableDnsRoundRobin = new ConfigurationProperty("enableDnsRoundRobin", typeof(bool), false, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty expect100Continue = new ConfigurationProperty("expect100Continue", typeof(bool), true, ConfigurationPropertyOptions.None);
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private readonly ConfigurationProperty useNagleAlgorithm = new ConfigurationProperty("useNagleAlgorithm", typeof(bool), true, ConfigurationPropertyOptions.None);

        public ServicePointManagerElement()
        {
            this.properties.Add(this.checkCertificateName);
            this.properties.Add(this.checkCertificateRevocationList);
            this.properties.Add(this.dnsRefreshTimeout);
            this.properties.Add(this.enableDnsRoundRobin);
            this.properties.Add(this.expect100Continue);
            this.properties.Add(this.useNagleAlgorithm);
        }

        protected override void PostDeserialize()
        {
            if (!base.EvaluationContext.IsMachineLevel)
            {
                PropertyInformation[] informationArray = new PropertyInformation[] { base.ElementInformation.Properties["checkCertificateName"], base.ElementInformation.Properties["checkCertificateRevocationList"] };
                foreach (PropertyInformation information in informationArray)
                {
                    if (information.ValueOrigin == PropertyValueOrigin.SetHere)
                    {
                        try
                        {
                            ExceptionHelper.UnmanagedPermission.Demand();
                        }
                        catch (Exception exception)
                        {
                            throw new ConfigurationErrorsException(System.SR.GetString("net_config_property_permission", new object[] { information.Name }), exception);
                        }
                    }
                }
            }
        }

        [ConfigurationProperty("checkCertificateName", DefaultValue=true)]
        public bool CheckCertificateName
        {
            get => 
                ((bool) base[this.checkCertificateName]);
            set
            {
                base[this.checkCertificateName] = value;
            }
        }

        [ConfigurationProperty("checkCertificateRevocationList", DefaultValue=false)]
        public bool CheckCertificateRevocationList
        {
            get => 
                ((bool) base[this.checkCertificateRevocationList]);
            set
            {
                base[this.checkCertificateRevocationList] = value;
            }
        }

        [ConfigurationProperty("dnsRefreshTimeout", DefaultValue=0x1d4c0)]
        public int DnsRefreshTimeout
        {
            get => 
                ((int) base[this.dnsRefreshTimeout]);
            set
            {
                base[this.dnsRefreshTimeout] = value;
            }
        }

        [ConfigurationProperty("enableDnsRoundRobin", DefaultValue=false)]
        public bool EnableDnsRoundRobin
        {
            get => 
                ((bool) base[this.enableDnsRoundRobin]);
            set
            {
                base[this.enableDnsRoundRobin] = value;
            }
        }

        [ConfigurationProperty("expect100Continue", DefaultValue=true)]
        public bool Expect100Continue
        {
            get => 
                ((bool) base[this.expect100Continue]);
            set
            {
                base[this.expect100Continue] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;

        [ConfigurationProperty("useNagleAlgorithm", DefaultValue=true)]
        public bool UseNagleAlgorithm
        {
            get => 
                ((bool) base[this.useNagleAlgorithm]);
            set
            {
                base[this.useNagleAlgorithm] = value;
            }
        }
    }
}

