namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;

    public sealed class ServiceHostingEnvironmentSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static ServiceHostingEnvironmentSection GetSection() => 
            ((ServiceHostingEnvironmentSection) ConfigurationHelpers.GetSection(ConfigurationStrings.ServiceHostingEnvironmentSectionPath));

        protected override void PostDeserialize()
        {
            if (!base.EvaluationContext.IsMachineLevel && (PropertyValueOrigin.SetHere == base.ElementInformation.Properties["minFreeMemoryPercentageToActivateService"].ValueOrigin))
            {
                try
                {
                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                }
                catch (SecurityException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("Hosting_MemoryGatesCheckFailedUnderPartialTrust")));
                }
            }
        }

        [SecurityCritical]
        internal static ServiceHostingEnvironmentSection UnsafeGetSection() => 
            ((ServiceHostingEnvironmentSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.ServiceHostingEnvironmentSectionPath));

        [ConfigurationProperty("aspNetCompatibilityEnabled", DefaultValue=false)]
        public bool AspNetCompatibilityEnabled
        {
            get => 
                ((bool) base["aspNetCompatibilityEnabled"]);
            set
            {
                base["aspNetCompatibilityEnabled"] = value;
            }
        }

        [ConfigurationProperty("baseAddressPrefixFilters", Options=ConfigurationPropertyOptions.None)]
        public BaseAddressPrefixFilterElementCollection BaseAddressPrefixFilters =>
            ((BaseAddressPrefixFilterElementCollection) base["baseAddressPrefixFilters"]);

        [IntegerValidator(MinValue=0, MaxValue=0x63), ConfigurationProperty("minFreeMemoryPercentageToActivateService", DefaultValue=5)]
        public int MinFreeMemoryPercentageToActivateService
        {
            get => 
                ((int) base["minFreeMemoryPercentageToActivateService"]);
            set
            {
                base["minFreeMemoryPercentageToActivateService"] = value;
            }
        }

        [ConfigurationProperty("multipleSiteBindingsEnabled", DefaultValue=false)]
        public bool MultipleSiteBindingsEnabled
        {
            get => 
                ((bool) base["multipleSiteBindingsEnabled"]);
            set
            {
                base["multipleSiteBindingsEnabled"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(TransportConfigurationTypeElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
                        new ConfigurationProperty("baseAddressPrefixFilters", typeof(BaseAddressPrefixFilterElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("aspNetCompatibilityEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("minFreeMemoryPercentageToActivateService", typeof(int), 5, null, new IntegerValidator(0, 0x63, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("multipleSiteBindingsEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public TransportConfigurationTypeElementCollection TransportConfigurationTypes =>
            ((TransportConfigurationTypeElementCollection) base[""]);
    }
}

