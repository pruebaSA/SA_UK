namespace System.ServiceModel.Configuration
{
    using System.Configuration;
    using System.ServiceModel;

    public sealed class ServiceModelSectionGroup : ConfigurationSectionGroup
    {
        public static ServiceModelSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
            }
            return (ServiceModelSectionGroup) config.SectionGroups["system.serviceModel"];
        }

        public BehaviorsSection Behaviors =>
            ((BehaviorsSection) base.Sections["behaviors"]);

        public BindingsSection Bindings =>
            ((BindingsSection) base.Sections["bindings"]);

        public ClientSection Client =>
            ((ClientSection) base.Sections["client"]);

        public ComContractsSection ComContracts =>
            ((ComContractsSection) base.Sections["comContracts"]);

        public CommonBehaviorsSection CommonBehaviors =>
            ((CommonBehaviorsSection) base.Sections["commonBehaviors"]);

        public DiagnosticSection Diagnostic =>
            ((DiagnosticSection) base.Sections["diagnostics"]);

        public ExtensionsSection Extensions =>
            ((ExtensionsSection) base.Sections["extensions"]);

        public ServiceHostingEnvironmentSection ServiceHostingEnvironment =>
            ((ServiceHostingEnvironmentSection) base.Sections["serviceHostingEnvironment"]);

        public ServicesSection Services =>
            ((ServicesSection) base.Sections["services"]);
    }
}

