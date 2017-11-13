namespace System.ServiceModel.PeerResolvers
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    public class PeerCustomResolverSettings
    {
        private EndpointAddress address;
        private System.ServiceModel.Channels.Binding binding;
        private string bindingConfiguration;
        private string bindingSection;
        private PeerResolver resolver;

        public EndpointAddress Address
        {
            get => 
                this.address;
            set
            {
                this.address = value;
            }
        }

        public System.ServiceModel.Channels.Binding Binding
        {
            get
            {
                if (((this.binding == null) && !string.IsNullOrEmpty(this.bindingSection)) && !string.IsNullOrEmpty(this.bindingConfiguration))
                {
                    this.binding = ConfigLoader.LookupBinding(this.bindingSection, this.bindingConfiguration);
                }
                return this.binding;
            }
            set
            {
                this.binding = value;
            }
        }

        internal string BindingConfiguration
        {
            get => 
                this.bindingConfiguration;
            set
            {
                this.bindingConfiguration = value;
            }
        }

        internal string BindingSection
        {
            get => 
                this.bindingSection;
            set
            {
                this.bindingSection = value;
            }
        }

        public bool IsBindingSpecified =>
            ((this.binding != null) || (!string.IsNullOrEmpty(this.bindingSection) && !string.IsNullOrEmpty(this.bindingConfiguration)));

        public PeerResolver Resolver
        {
            get => 
                this.resolver;
            set
            {
                this.resolver = value;
            }
        }
    }
}

