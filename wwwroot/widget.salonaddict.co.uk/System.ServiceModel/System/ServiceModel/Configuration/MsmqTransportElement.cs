namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class MsmqTransportElement : MsmqElementBase
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            MsmqTransportBindingElement element = bindingElement as MsmqTransportBindingElement;
            element.MaxPoolSize = this.MaxPoolSize;
            element.QueueTransferProtocol = this.QueueTransferProtocol;
            element.UseActiveDirectory = this.UseActiveDirectory;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            MsmqTransportElement element = from as MsmqTransportElement;
            if (element != null)
            {
                this.MaxPoolSize = element.MaxPoolSize;
                this.QueueTransferProtocol = element.QueueTransferProtocol;
                this.UseActiveDirectory = element.UseActiveDirectory;
            }
        }

        protected override TransportBindingElement CreateDefaultBindingElement() => 
            new MsmqTransportBindingElement();

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            MsmqTransportBindingElement element = bindingElement as MsmqTransportBindingElement;
            this.MaxPoolSize = element.MaxPoolSize;
            this.QueueTransferProtocol = element.QueueTransferProtocol;
            this.UseActiveDirectory = element.UseActiveDirectory;
        }

        public override Type BindingElementType =>
            typeof(MsmqTransportBindingElement);

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxPoolSize", DefaultValue=8)]
        public int MaxPoolSize
        {
            get => 
                ((int) base["maxPoolSize"]);
            set
            {
                base["maxPoolSize"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("maxPoolSize", typeof(int), 8, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("queueTransferProtocol", typeof(System.ServiceModel.QueueTransferProtocol), System.ServiceModel.QueueTransferProtocol.Native, null, new ServiceModelEnumValidator(typeof(QueueTransferProtocolHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("useActiveDirectory", typeof(bool), false, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ServiceModelEnumValidator(typeof(QueueTransferProtocolHelper)), ConfigurationProperty("queueTransferProtocol", DefaultValue=0)]
        public System.ServiceModel.QueueTransferProtocol QueueTransferProtocol
        {
            get => 
                ((System.ServiceModel.QueueTransferProtocol) base["queueTransferProtocol"]);
            set
            {
                base["queueTransferProtocol"] = value;
            }
        }

        [ConfigurationProperty("useActiveDirectory", DefaultValue=false)]
        public bool UseActiveDirectory
        {
            get => 
                ((bool) base["useActiveDirectory"]);
            set
            {
                base["useActiveDirectory"] = value;
            }
        }
    }
}

