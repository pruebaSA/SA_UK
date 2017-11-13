namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Net;
    using System.ServiceModel.Channels;

    public class PeerTransportElement : BindingElementExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            PeerTransportBindingElement element = (PeerTransportBindingElement) bindingElement;
            element.ListenIPAddress = this.ListenIPAddress;
            element.Port = this.Port;
            element.MaxBufferPoolSize = this.MaxBufferPoolSize;
            element.MaxReceivedMessageSize = this.MaxReceivedMessageSize;
            this.Security.ApplyConfiguration(element.Security);
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            PeerTransportElement element = (PeerTransportElement) from;
            this.ListenIPAddress = element.ListenIPAddress;
            this.Port = element.Port;
            this.MaxBufferPoolSize = element.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = element.MaxReceivedMessageSize;
            this.Security.CopyFrom(element.Security);
        }

        protected internal override BindingElement CreateBindingElement()
        {
            PeerTransportBindingElement bindingElement = new PeerTransportBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            PeerTransportBindingElement element = (PeerTransportBindingElement) bindingElement;
            this.ListenIPAddress = element.ListenIPAddress;
            this.Port = element.Port;
            this.MaxBufferPoolSize = element.MaxBufferPoolSize;
            this.MaxReceivedMessageSize = element.MaxReceivedMessageSize;
            this.Security.InitializeFrom(element.Security);
        }

        public override Type BindingElementType =>
            typeof(PeerTransportBindingElement);

        [TypeConverter(typeof(PeerTransportListenAddressConverter)), PeerTransportListenAddressValidator, ConfigurationProperty("listenIPAddress", DefaultValue=null)]
        public IPAddress ListenIPAddress
        {
            get => 
                ((IPAddress) base["listenIPAddress"]);
            set
            {
                base["listenIPAddress"] = value;
            }
        }

        [LongValidator(MinValue=1L), ConfigurationProperty("maxBufferPoolSize", DefaultValue=0x80000L)]
        public long MaxBufferPoolSize
        {
            get => 
                ((long) base["maxBufferPoolSize"]);
            set
            {
                base["maxBufferPoolSize"] = value;
            }
        }

        [ConfigurationProperty("maxReceivedMessageSize", DefaultValue=0x10000L), LongValidator(MinValue=1L)]
        public long MaxReceivedMessageSize
        {
            get => 
                ((long) base["maxReceivedMessageSize"]);
            set
            {
                base["maxReceivedMessageSize"] = value;
            }
        }

        [ConfigurationProperty("port", DefaultValue=0), IntegerValidator(MinValue=0, MaxValue=0xffff)]
        public int Port
        {
            get => 
                ((int) base["port"]);
            set
            {
                base["port"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("listenIPAddress", typeof(IPAddress), null, new PeerTransportListenAddressConverter(), new PeerTransportListenAddressValidator(), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxBufferPoolSize", typeof(long), 0x80000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxReceivedMessageSize", typeof(long), 0x10000L, null, new LongValidator(1L, 0x7fffffffffffffffL, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("port", typeof(int), 0, null, new IntegerValidator(0, 0xffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("security", typeof(PeerSecurityElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("security")]
        public PeerSecurityElement Security =>
            ((PeerSecurityElement) base["security"]);
    }
}

