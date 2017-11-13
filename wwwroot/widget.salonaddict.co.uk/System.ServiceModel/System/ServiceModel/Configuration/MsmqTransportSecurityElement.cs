namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Net.Security;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class MsmqTransportSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(MsmqTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.MsmqAuthenticationMode = this.MsmqAuthenticationMode;
            security.MsmqEncryptionAlgorithm = this.MsmqEncryptionAlgorithm;
            security.MsmqProtectionLevel = this.MsmqProtectionLevel;
            security.MsmqSecureHashAlgorithm = this.MsmqSecureHashAlgorithm;
        }

        internal void InitializeFrom(MsmqTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.MsmqAuthenticationMode = security.MsmqAuthenticationMode;
            this.MsmqEncryptionAlgorithm = security.MsmqEncryptionAlgorithm;
            this.MsmqProtectionLevel = security.MsmqProtectionLevel;
            this.MsmqSecureHashAlgorithm = security.MsmqSecureHashAlgorithm;
        }

        [ServiceModelEnumValidator(typeof(MsmqAuthenticationModeHelper)), ConfigurationProperty("msmqAuthenticationMode", DefaultValue=1)]
        public System.ServiceModel.MsmqAuthenticationMode MsmqAuthenticationMode
        {
            get => 
                ((System.ServiceModel.MsmqAuthenticationMode) base["msmqAuthenticationMode"]);
            set
            {
                base["msmqAuthenticationMode"] = value;
            }
        }

        [ServiceModelEnumValidator(typeof(MsmqEncryptionAlgorithmHelper)), ConfigurationProperty("msmqEncryptionAlgorithm", DefaultValue=0)]
        public System.ServiceModel.MsmqEncryptionAlgorithm MsmqEncryptionAlgorithm
        {
            get => 
                ((System.ServiceModel.MsmqEncryptionAlgorithm) base["msmqEncryptionAlgorithm"]);
            set
            {
                base["msmqEncryptionAlgorithm"] = value;
            }
        }

        [ConfigurationProperty("msmqProtectionLevel", DefaultValue=1), ServiceModelEnumValidator(typeof(ProtectionLevelHelper))]
        public ProtectionLevel MsmqProtectionLevel
        {
            get => 
                ((ProtectionLevel) base["msmqProtectionLevel"]);
            set
            {
                base["msmqProtectionLevel"] = value;
            }
        }

        [ConfigurationProperty("msmqSecureHashAlgorithm", DefaultValue=1), ServiceModelEnumValidator(typeof(MsmqSecureHashAlgorithmHelper))]
        public System.ServiceModel.MsmqSecureHashAlgorithm MsmqSecureHashAlgorithm
        {
            get => 
                ((System.ServiceModel.MsmqSecureHashAlgorithm) base["msmqSecureHashAlgorithm"]);
            set
            {
                base["msmqSecureHashAlgorithm"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("msmqAuthenticationMode", typeof(System.ServiceModel.MsmqAuthenticationMode), System.ServiceModel.MsmqAuthenticationMode.WindowsDomain, null, new ServiceModelEnumValidator(typeof(MsmqAuthenticationModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("msmqEncryptionAlgorithm", typeof(System.ServiceModel.MsmqEncryptionAlgorithm), System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream, null, new ServiceModelEnumValidator(typeof(MsmqEncryptionAlgorithmHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("msmqProtectionLevel", typeof(ProtectionLevel), ProtectionLevel.Sign, null, new ServiceModelEnumValidator(typeof(ProtectionLevelHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("msmqSecureHashAlgorithm", typeof(System.ServiceModel.MsmqSecureHashAlgorithm), System.ServiceModel.MsmqSecureHashAlgorithm.Sha1, null, new ServiceModelEnumValidator(typeof(MsmqSecureHashAlgorithmHelper)), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

