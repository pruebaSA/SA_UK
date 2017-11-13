namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;

    public sealed class ServicesSection : ConfigurationSection, IConfigurationContextProviderInternal
    {
        [SecurityCritical]
        private EvaluationContextHelper contextHelper;
        private ConfigurationPropertyCollection properties;

        internal static ServicesSection GetSection() => 
            ((ServicesSection) ConfigurationHelpers.GetSection(ConfigurationStrings.ServicesSectionPath));

        protected override void PostDeserialize()
        {
            this.ValidateSection();
            base.PostDeserialize();
        }

        [SecurityCritical]
        protected override void Reset(ConfigurationElement parentElement)
        {
            this.contextHelper.OnReset(parentElement);
            base.Reset(parentElement);
        }

        ContextInformation IConfigurationContextProviderInternal.GetEvaluationContext() => 
            base.EvaluationContext;

        [SecurityCritical]
        ContextInformation IConfigurationContextProviderInternal.GetOriginalEvaluationContext() => 
            this.contextHelper.GetOriginalContext(this);

        [SecurityCritical]
        internal static ServicesSection UnsafeGetSection() => 
            ((ServicesSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.ServicesSectionPath));

        private void ValidateSection()
        {
            ContextInformation evaluationContext = ConfigurationHelpers.GetEvaluationContext(this);
            if (evaluationContext != null)
            {
                foreach (ServiceElement element in this.Services)
                {
                    BehaviorsSection.ValidateServiceBehaviorReference(element.BehaviorConfiguration, evaluationContext, element);
                    foreach (ServiceEndpointElement element2 in element.Endpoints)
                    {
                        BehaviorsSection.ValidateEndpointBehaviorReference(element2.BehaviorConfiguration, evaluationContext, element2);
                        BindingsSection.ValidateBindingReference(element2.Binding, element2.BindingConfiguration, evaluationContext, element2);
                    }
                }
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(ServiceElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ServiceElementCollection Services =>
            ((ServiceElementCollection) base[""]);
    }
}

