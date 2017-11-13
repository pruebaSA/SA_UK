namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.Security;

    public class CommonBehaviorsSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static CommonBehaviorsSection GetSection() => 
            ((CommonBehaviorsSection) ConfigurationHelpers.GetSection(ConfigurationStrings.CommonBehaviorsSectionPath));

        [SecurityCritical]
        internal static CommonBehaviorsSection UnsafeGetAssociatedSection(ContextInformation contextEval) => 
            ((CommonBehaviorsSection) ConfigurationHelpers.UnsafeGetAssociatedSection(contextEval, ConfigurationStrings.CommonBehaviorsSectionPath));

        [SecurityCritical]
        internal static CommonBehaviorsSection UnsafeGetSection() => 
            ((CommonBehaviorsSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.CommonBehaviorsSectionPath));

        [ConfigurationProperty("endpointBehaviors", Options=ConfigurationPropertyOptions.None)]
        public CommonEndpointBehaviorElement EndpointBehaviors =>
            ((CommonEndpointBehaviorElement) base["endpointBehaviors"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("endpointBehaviors", typeof(CommonEndpointBehaviorElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("serviceBehaviors", typeof(CommonServiceBehaviorElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("serviceBehaviors", Options=ConfigurationPropertyOptions.None)]
        public CommonServiceBehaviorElement ServiceBehaviors =>
            ((CommonServiceBehaviorElement) base["serviceBehaviors"]);
    }
}

