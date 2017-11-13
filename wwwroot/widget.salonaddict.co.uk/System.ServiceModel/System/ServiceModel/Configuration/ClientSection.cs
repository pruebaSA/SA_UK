namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;

    public sealed class ClientSection : ConfigurationSection, IConfigurationContextProviderInternal
    {
        private ConfigurationPropertyCollection properties;

        internal static ClientSection GetSection() => 
            ((ClientSection) ConfigurationHelpers.GetSection(ConfigurationStrings.ClientSectionPath));

        protected override void InitializeDefault()
        {
            this.Metadata.SetDefaults();
        }

        protected override void PostDeserialize()
        {
            this.ValidateSection();
            base.PostDeserialize();
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            null;

        [SecurityCritical]
        internal static ClientSection UnsafeGetSection() => 
            ((ClientSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.ClientSectionPath));

        [SecurityCritical]
        internal static ClientSection UnsafeGetSection(ContextInformation contextInformation) => 
            ((ClientSection) ConfigurationHelpers.UnsafeGetSectionFromContext(contextInformation, ConfigurationStrings.ClientSectionPath));

        private void ValidateSection()
        {
            ContextInformation evaluationContext = ConfigurationHelpers.GetEvaluationContext(this);
            if (evaluationContext != null)
            {
                foreach (ChannelEndpointElement element in this.Endpoints)
                {
                    BehaviorsSection.ValidateEndpointBehaviorReference(element.BehaviorConfiguration, evaluationContext, element);
                    BindingsSection.ValidateBindingReference(element.Binding, element.BindingConfiguration, evaluationContext, element);
                }
            }
        }

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ChannelEndpointElementCollection Endpoints =>
            ((ChannelEndpointElementCollection) base[""]);

        [ConfigurationProperty("metadata")]
        public MetadataElement Metadata =>
            ((MetadataElement) base["metadata"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(ChannelEndpointElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection),
                        new ConfigurationProperty("metadata", typeof(MetadataElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

