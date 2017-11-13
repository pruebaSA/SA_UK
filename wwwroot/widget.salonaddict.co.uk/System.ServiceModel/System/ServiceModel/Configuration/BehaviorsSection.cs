namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel;

    public class BehaviorsSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static BehaviorsSection GetSection() => 
            ((BehaviorsSection) ConfigurationHelpers.GetSection(ConfigurationStrings.BehaviorsSectionPath));

        [SecurityCritical]
        internal static BehaviorsSection UnsafeGetAssociatedSection(ContextInformation evalContext) => 
            ((BehaviorsSection) ConfigurationHelpers.UnsafeGetAssociatedSection(evalContext, ConfigurationStrings.BehaviorsSectionPath));

        [SecurityCritical]
        internal static BehaviorsSection UnsafeGetSection() => 
            ((BehaviorsSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.BehaviorsSectionPath));

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void ValidateEndpointBehaviorReference(string behaviorConfiguration, ContextInformation evaluationContext, ConfigurationElement configurationElement)
        {
            if (evaluationContext == null)
            {
                DiagnosticUtility.FailFast("ValidateBehaviorReference() should only called with valid ContextInformation");
            }
            if (!string.IsNullOrEmpty(behaviorConfiguration))
            {
                BehaviorsSection section = (BehaviorsSection) ConfigurationHelpers.UnsafeGetAssociatedSection(evaluationContext, ConfigurationStrings.BehaviorsSectionPath);
                if (!section.EndpointBehaviors.ContainsKey(behaviorConfiguration))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidEndpointBehavior", new object[] { behaviorConfiguration }), configurationElement.ElementInformation.Source, configurationElement.ElementInformation.LineNumber));
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void ValidateServiceBehaviorReference(string behaviorConfiguration, ContextInformation evaluationContext, ConfigurationElement configurationElement)
        {
            if (evaluationContext == null)
            {
                DiagnosticUtility.FailFast("ValidateBehaviorReference() should only called with valid ContextInformation");
            }
            if (!string.IsNullOrEmpty(behaviorConfiguration))
            {
                BehaviorsSection section = (BehaviorsSection) ConfigurationHelpers.UnsafeGetAssociatedSection(evaluationContext, ConfigurationStrings.BehaviorsSectionPath);
                if (!section.ServiceBehaviors.ContainsKey(behaviorConfiguration))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidServiceBehavior", new object[] { behaviorConfiguration }), configurationElement.ElementInformation.Source, configurationElement.ElementInformation.LineNumber));
                }
            }
        }

        [ConfigurationProperty("endpointBehaviors", Options=ConfigurationPropertyOptions.None)]
        public EndpointBehaviorElementCollection EndpointBehaviors =>
            ((EndpointBehaviorElementCollection) base["endpointBehaviors"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("endpointBehaviors", typeof(EndpointBehaviorElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("serviceBehaviors", typeof(ServiceBehaviorElementCollection), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("serviceBehaviors", Options=ConfigurationPropertyOptions.None)]
        public ServiceBehaviorElementCollection ServiceBehaviors =>
            ((ServiceBehaviorElementCollection) base["serviceBehaviors"]);
    }
}

