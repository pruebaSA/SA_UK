namespace System.ServiceModel.Activation.Configuration
{
    using System.Configuration;
    using System.ServiceModel;

    public sealed class ServiceModelActivationSectionGroup : ConfigurationSectionGroup
    {
        public static ServiceModelActivationSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
            }
            return (ServiceModelActivationSectionGroup) config.SectionGroups["system.serviceModel.activation"];
        }

        public DiagnosticSection Diagnostics =>
            ((DiagnosticSection) base.Sections["diagnostics"]);

        public NetPipeSection NetPipe =>
            ((NetPipeSection) base.Sections["net.pipe"]);

        public NetTcpSection NetTcp =>
            ((NetTcpSection) base.Sections["net.tcp"]);
    }
}

