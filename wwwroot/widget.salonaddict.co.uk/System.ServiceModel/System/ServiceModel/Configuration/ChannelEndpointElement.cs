namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel;

    public sealed class ChannelEndpointElement : ConfigurationElement, IConfigurationContextProviderInternal
    {
        [SecurityCritical]
        private EvaluationContextHelper contextHelper;
        private ConfigurationPropertyCollection properties;

        public ChannelEndpointElement()
        {
        }

        public ChannelEndpointElement(EndpointAddress address, string contractType) : this()
        {
            if (address != null)
            {
                this.Address = address.Uri;
                this.Headers.Headers = address.Headers;
                if (address.Identity != null)
                {
                    this.Identity.InitializeFrom(address.Identity);
                }
            }
            this.Contract = contractType;
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

        [ConfigurationProperty("address", Options=ConfigurationPropertyOptions.None)]
        public Uri Address
        {
            get => 
                ((Uri) base["address"]);
            set
            {
                base["address"] = value;
            }
        }

        [ConfigurationProperty("behaviorConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string BehaviorConfiguration
        {
            get => 
                ((string) base["behaviorConfiguration"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["behaviorConfiguration"] = value;
            }
        }

        [ConfigurationProperty("binding", Options=ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Binding
        {
            get => 
                ((string) base["binding"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["binding"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("bindingConfiguration", DefaultValue="")]
        public string BindingConfiguration
        {
            get => 
                ((string) base["bindingConfiguration"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["bindingConfiguration"] = value;
            }
        }

        [ConfigurationProperty("contract", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Contract
        {
            get => 
                ((string) base["contract"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["contract"] = value;
            }
        }

        [ConfigurationProperty("headers")]
        public AddressHeaderCollectionElement Headers =>
            ((AddressHeaderCollectionElement) base["headers"]);

        [ConfigurationProperty("identity")]
        public IdentityElement Identity =>
            ((IdentityElement) base["identity"]);

        [ConfigurationProperty("name", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey), StringValidator(MinLength=0)]
        public string Name
        {
            get => 
                ((string) base["name"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["name"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("address", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("behaviorConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("binding", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("bindingConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("contract", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("headers", typeof(AddressHeaderCollectionElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("identity", typeof(IdentityElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("name", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

